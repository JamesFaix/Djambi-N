import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import Controller from "../controllers/controller";
import { NotificationStrategy } from "./notifications";

class WebSocketClient {
    private readonly socket : WebSocket;
    private readonly shouldLog : () => boolean;

    constructor(
        shouldLog : () => boolean
    ) {
        this.shouldLog = shouldLog;

        const url = `${Environment.apiAddress()}/notifications/ws`
            .replace("http:", "ws:");
        const s = new WebSocket(url);
        s.onopen = () => this.onOpen();
        s.onclose = () => this.onClose();
        s.onmessage = e => this.onMessage(e);
        this.socket = s;
    }

    private onOpen() {
        if (this.shouldLog()) {
            console.log("WebSocket open");
        }
    }

    private onMessage(e : MessageEvent) {
        if (this.shouldLog()) {
            console.log("WebSocket message");
        }

        const updateJson = e.data as string;
        const update = JSON.parse(updateJson) as StateAndEventResponse;
        Controller.Game.onGameUpdateReceived(update);
    }

    private onClose() {
        if (this.shouldLog()) {
            console.log("WebSocket close");
        }
    }
}

export class WebSocketStrategy implements NotificationStrategy {
    private client : WebSocketClient = null;
    private shouldLog : () => boolean;

    constructor(shouldLog) {
        this.shouldLog = shouldLog;
    }

    public connect() {
        if (this.client) {
            this.disconnect();
        }

        this.client = new WebSocketClient(this.shouldLog);
    }

    public disconnect() {
        if (this.client) {
       //     WebSocketClientManager.client.dispose();
            this.client = null;
        }
    }
}