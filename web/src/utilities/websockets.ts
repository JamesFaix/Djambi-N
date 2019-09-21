import Environment from "../environment";
import { StateAndEventResponse } from "../api/model";
import Controller from "../controllers/controller";

class WebSocketClient {
    private readonly socket : WebSocket;
    private readonly shouldLog : () => boolean;

    constructor(
        shouldLog : () => boolean
    ) {
        this.shouldLog = shouldLog;

        const url = `${Environment.apiAddress()}/notifications`
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

export class WebSocketClientManager {
    private static client : WebSocketClient = null;
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

        if (WebSocketClientManager.client) {
            this.disconnect();
        }

        this.client = new WebSocketClient(this.shouldLog);
    }

    public static disconnect() {
        if (!this.isInit) {
            throw "Cannot disconnect if not initilaized.";
        }

        if (WebSocketClientManager.client) {
       //     WebSocketClientManager.client.dispose();
            WebSocketClientManager.client = null;
        }
    }
}