import { Event, EventKind, Effect, EffectKind } from "../api/model";

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

    public static isDisplayableEffect(effect : Effect) : boolean {
        switch(effect.kind) {
            case EffectKind.PieceAbandoned:
            case EffectKind.PieceDropped:
            case EffectKind.PieceEnlisted:
            case EffectKind.PieceKilled:
            case EffectKind.PieceMoved:
            case EffectKind.PieceVacated:
            case EffectKind.PlayerOutOfMoves:
            case EffectKind.PlayerStatusChanged:
            case EffectKind.TurnCyclePlayerFellFromPower:
            case EffectKind.TurnCyclePlayerRoseToPower:
                return true;

            default:
                return false;
        }
    }
}