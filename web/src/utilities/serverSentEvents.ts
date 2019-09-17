import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import Controller from "../controllers/controller";
import { NotificationType } from "../store/notifications";

class SseClient {
    private readonly source : EventSource;
    private readonly shouldLog : () => boolean;

    constructor(
        shouldLog : () => boolean
    ) {
        this.shouldLog = shouldLog;

        const s = new EventSource(
            Environment.apiAddress() + "/notifications",
            { withCredentials: true }
        );
        s.onopen = e => this.onOpen(e);
        s.onmessage = e => this.onMessage(e);
        s.onerror = e => this.onError(e);
        this.source = s;
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
        Controller.Game.onGameUpdateReceived(update);
    }

    private onError(e : Event) {
        Controller.addNotification(NotificationType.Error, "Server-sent event error");
        if (this.shouldLog()) {
            console.log("SSE Error");
            console.log(e);
        }
    }
}

export class SseClientManager {
    private static client : SseClient = null;
    private static isInit : boolean = false;
    private static shouldLog : () => boolean;

    public static init(
        shouldLog : () => boolean
    ) : void {
        if (this.isInit) {
            throw "Cannot initialize more than once.";
        }

        this.isInit = true;
        this.shouldLog = shouldLog;
    }

    public static connect() {
        if (!this.isInit) {
            throw "Cannot connect if not initilaized.";
        }

        if (SseClientManager.client) {
            this.disconnect();
        }

        this.client = new SseClient(this.shouldLog);
    }

    public static disconnect() {
        if (!this.isInit) {
            throw "Cannot disconnect if not initilaized.";
        }

        if (SseClientManager.client) {
            SseClientManager.client.dispose();
            SseClientManager.client = null;
        }
    }
}