import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import Controller from "../controllers/controller";
import { NotificationStrategy, LogSink } from "./notifications";

class WebSocketClient {
    private readonly socket : WebSocket;
    private readonly log : LogSink;

    constructor(log : LogSink) {
        this.log = log;

        const url = `${Environment.apiAddress()}/notifications/ws`
            .replace("http:", "ws:");
        const s = new WebSocket(url);
        s.onopen = () => this.onOpen();
        s.onclose = () => this.onClose();
        s.onmessage = e => this.onMessage(e);
        this.socket = s;
    }

    public dispose() : void {
        this.socket.close();
    }

    private onOpen() {
        this.log("WebSocket open");
    }

    private onMessage(e : MessageEvent) {
        this.log("WebSocket message");
        const updateJson = e.data as string;
        const update = JSON.parse(updateJson) as StateAndEventResponse;
        Controller.Game.onGameUpdateReceived(update);
    }

    private onClose() {
        this.log("WebSocket closed");
    }
}

export class WebSocketsStrategy implements NotificationStrategy {
    private client : WebSocketClient = null;
    private readonly log : LogSink;

    constructor(log : LogSink) {
        this.log = log;
    }

    public connect() {
        if (this.client) {
            this.disconnect();
        }

        this.client = new WebSocketClient(this.log);
    }

    public disconnect() {
        if (this.client) {
            this.client.dispose();
            this.client = null;
        }
    }
}