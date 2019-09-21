import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import Controller from "../controllers/controller";
import { NotificationType } from "../store/notifications";
import { NotificationStrategy, LogSink } from "./notifications";

class SseClient {
    private readonly source : EventSource;
    private readonly log : LogSink;

    constructor(log : LogSink) {
        this.log = log;

        const s = new EventSource(
            Environment.apiAddress() + "/notifications/sse",
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
        this.log("SSE Open");
    }

    private onMessage(e : MessageEvent) {
        this.log("SSE Message");
        const updateJson = e.data as string;
        const update = JSON.parse(updateJson) as StateAndEventResponse;
        Controller.Game.onGameUpdateReceived(update);
    }

    private onError(e : Event) {
        Controller.addNotification(NotificationType.Error, "Server-sent event error");
        this.log("SSE Error");
    }
}

export class ServerSentEventsStrategy implements NotificationStrategy {
    private client : SseClient = null;
    private readonly log : LogSink;

    constructor(log : LogSink){
        this.log = log;
    }

    public connect() {
        if (this.client) {
            this.disconnect();
        }

        this.client = new SseClient(this.log);
    }

    public disconnect() {
        if (this.client) {
            this.client.dispose();
            this.client = null;
        }
    }
}