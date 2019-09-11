import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import { Store } from "redux";
import { State, CustomAction, AppStore, getAppState } from "../store/root";
import Controller from "../controller";

class SseClient {
    private readonly source : EventSource;
    private readonly store : AppStore;

    constructor(
        store : AppStore
    ) {
        this.store = store;

        const s = new EventSource(
            Environment.apiAddress() + "/notifications",
            { withCredentials: true }
        );
        s.onopen = e => this.onOpen(e);
        s.onmessage = e => this.onMessage(e);
        s.onerror = e => this.onError(e);
        this.source = s;
    }

    private shouldLog() : boolean {
        return getAppState(this.store).settings.debug.logSse;
    }

    dispose() : void {
        this.source.close();
    }

    private onOpen(e : Event) {
        if (this.shouldLog()) {
            console.log("SSE Open");
        }
    }

    private onMessage(e : MessageEvent) {
        if (this.shouldLog()) {
            console.log("SSE Message");
        }

        const updateJson = e.data as string;
        const update = JSON.parse(updateJson) as StateAndEventResponse;

        const state = this.store.getState();

        const activeGame = state.activeGame.game;
        const activeGameId = activeGame ? activeGame.id : null;

        if (update.game.id === activeGameId) {
            Controller.Game.updateInProgressGame(update);
        } else {
            console.log(`An event occurred in game ${update.game.id}`);
            //TODO: Toast notification
        }
    }

    private onError(e : Event) {
     // if (this.shouldLog()) {
            console.log("SSE Error");
            console.log(e);
      //  }

        //TODO: dispatch server error action
    }
}

export class SseClientManager {
    private static client : SseClient = null;
    private static store : Store<State, CustomAction> = null;

    public static init(store : Store<State, CustomAction>) : void {
        if (SseClientManager.store) {
            throw "Cannot initialize more than once.";
        }

        SseClientManager.store = store;
    }

    public static connect() {
        if (!SseClientManager.store) {
            throw "Cannot connect if not initilaized.";
        }

        if (SseClientManager.client) {
            this.disconnect();
        }

        this.client = new SseClient(this.store);
    }

    public static disconnect() {
        if (!SseClientManager.store) {
            throw "Cannot disconnect if not initilaized.";
        }

        if (SseClientManager.client) {
            SseClientManager.client.dispose();
            SseClientManager.client = null;
        }
    }
}