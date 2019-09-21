import { WebSocketsStrategy } from "./websockets";
import { ServerSentEventsStrategy } from "./serverSentEvents";

export type LogSink = (msg : string) => void;

export enum NotificationStrategyType {
    WebSockets,
    ServerSentEvents
}

export interface NotificationStrategy {
    connect : () => void,
    disconnect : () => void
}

export default class NotificationsService {
    private static log : LogSink = null;
    private static strategy : NotificationStrategy = null;

    public static init(
        strategy : NotificationStrategyType,
        shouldLog : () => boolean
    ) : void {
        if (this.strategy) { throw "Cannot initialize more than once."; }

        this.log = (msg : string) => {
            if (shouldLog()) {
                console.log(msg);
            }
        };

        //Requires log is already initialized!
        this.strategy = NotificationsService.getStrategy(strategy);
    }

    private static getStrategy(type : NotificationStrategyType) : NotificationStrategy {
        switch (type) {
            case NotificationStrategyType.ServerSentEvents:
                return new ServerSentEventsStrategy(this.log);
            case NotificationStrategyType.WebSockets:
                return new WebSocketsStrategy(this.log);
            default:
                throw "Unsupported notification strategy type: " + type;
        }
    }

    public static connect() {
        if (!this.strategy) { throw "Cannot connect if not initilaized."; }
        this.strategy.connect();
    }

    public static disconnect() {
        if (!this.strategy) { throw "Cannot disconnect if not initilaized."; }
        this.strategy.disconnect();
    }
}