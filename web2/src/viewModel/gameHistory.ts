import { Event, EventKind } from "../api/model";

export default class GameHistory {
    public static isDisplayableEvent(event : Event) : boolean {
        switch (event.kind) {
            case EventKind.GameStarted:
            case EventKind.PlayerStatusChanged:
            case EventKind.TurnCommitted:
                return true;

            default:
                return false;
        }
    }
}