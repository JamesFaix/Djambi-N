export interface NotificationStrategy {
    connect : () => void,
    disconnect : () => void
}

export default class NotificationsService {
    private static shouldLog : () => boolean;
    private static isInit : boolean = false;
    private static strategy : NotificationStrategy = null;

    public static init(
        strategy : NotificationStrategy,
        shouldLog : () => boolean
    ) : void {
        if (this.isInit) { throw "Cannot initialize more than once."; }

        this.isInit = true;
        this.strategy = strategy;
        this.shouldLog = shouldLog;
    }

    public static connect() {
        if (!this.isInit) { throw "Cannot connect if not initilaized."; }
        this.strategy.connect();
    }

    public static disconnect() {
        if (!this.isInit) { throw "Cannot disconnect if not initilaized."; }
        this.strategy.disconnect();
    }
}