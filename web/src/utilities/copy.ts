import { AppStore, getAppState } from "../store/root";
import {
    Turn,
    Selection,
    Game,
    TurnStatus,
    SelectionKind,
    Location,
    Piece,
    PieceKind,
    Board,
    Effect,
    Event,
    EventKind,
    EffectKind,
    PlayerStatusChangedEffect,
    PlayerStatus,
    PieceAbandonedEffect,
    PieceDroppedEffect,
    PieceEnlistedEffect,
    PieceKilledEffect,
    PieceMovedEffect,
    PieceVacatedEffect,
    PlayerOutOfMovesEffect,
    TurnCyclePlayerFellFromPowerEffect,
    TurnCyclePlayerRoseToPowerEffect
} from "../api/model";
import { CellView, PieceView } from "../viewModel/board/model";
import ThemeService from "../themes/themeService";
import { Theme } from "../themes/model";

export default class Copy {
    private static store : AppStore

    public static init(store : AppStore) : void {
        if (!store) throw "Cannot initialize if arguments are null.";
        if (this.store) throw "Cannot initialize twice.";

        this.store = store;
    }

    private static getTheme() : Theme {
        return getAppState(this.store).display.theme;
    }

    private static showIds() : boolean {
        return getAppState(this.store).settings.debug.showCellAndPieceIds;
    }

    private static centerCellName() : string {
        return this.getTheme().copy.centerCellName;
    }

    public static boolToYesOrNo(value : boolean) : string {
        if (value === true) {
            return "Yes";
        } else if (value === false) {
            return "No"
        } else { //null/undefined
            return null;
        }
    }

    private static locationToString(location : Location) : string {
        return `(${location.region}, ${location.x}, ${location.y})`;
    }

    public static getCellLabel(cellId : number, board : Board) : string {
        //The first time the board is needed it must be fetched from the API.
        //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
        if (!board) {
            return cellId.toString();
        }

        const cell = board.cells.find(c => c.id === cellId);

        const base = cell.locations.find(l => l.x === 0 && l.y === 0)
            ? this.centerCellName()
            : Copy.locationToString(cell.locations[0]);

        if (this.showIds()){
            return `${base} (#${cellId})`;
        } else {
            return base;
        }
    }

    public static getCellViewLabel(cell : CellView) : string {
        const base = cell.locations.find(l => l.x === 0 && l.y === 0)
            ? this.centerCellName()
            : Copy.locationToString(cell.locations[0]);

        if (!this.showIds()) {
            return base;
        } else {
            return `${base} (#${cell.id})`;
        }
    }

    private static getPieceLabel(piece : Piece, game : Game) : string {
        const kindName = ThemeService.getPieceName(this.getTheme(), piece.kind);

        if (piece.kind === PieceKind.Corpse) {
            return kindName;
        }

        const player = game.players.find(p => p.id === piece.playerId);
        const base = player
            ? `${player.name}'s ${kindName}`
            : `Neutral ${kindName}`;

        if (this.showIds()){
            return `${base} (#${piece.id})`;
        } else {
            return base;
        }
    }

    public static getPieceViewLabel(piece : PieceView, game : Game) {
        const kindName = ThemeService.getPieceName(this.getTheme(), piece.kind);

        if (piece.kind === PieceKind.Corpse) {
            return kindName;
        }

        const base = piece.playerName
            ? `${piece.playerName}'s ${kindName}`
            : `Neutral ${kindName}`;

        if (this.showIds()){
            return `${base} (#${piece.id})`;
        } else {
            return base;
        }
    }

    public static getTurnPrompt(turn : Turn) : string {
        switch (turn.status) {
            case TurnStatus.AwaitingSelection:
                return Copy.getSelectionPrompt(turn.requiredSelectionKind);
            case TurnStatus.AwaitingCommit:
                return "End your turn or reset.";
            case TurnStatus.DeadEnd:
                return "No futher selections are available. You must reset.";
            default:
                throw "Invalid turn status.";
        }
    }

    private static getSelectionPrompt(kind : SelectionKind) : string {
        switch (kind) {
            case SelectionKind.Drop: return "Select a cell to drop the target piece in.";
            case SelectionKind.Move: return "Select a cell to move to.";
            case SelectionKind.Subject: return "Select a piece to move.";
            case SelectionKind.Target: return "Select a piece to target.";
            case SelectionKind.Vacate: return "Select a cell to vacate to.";
            default: throw "Invalid selection kind.";
        }
    }

    public static getSelectionDescription(selection : Selection, game : Game, board : Board) : string {
        const cell = selection.cellId
            ? Copy.getCellLabel(selection.cellId, board)
            : null;

        const piece = selection.pieceId
            ? Copy.getPieceLabel(game.pieces.find(p => p.id === selection.pieceId), game)
            : null;

        switch (selection.kind) {
            case SelectionKind.Drop:
                return `Drop target piece at cell ${cell}.`;

            case SelectionKind.Move:
                if (piece === null) {
                    return `Move to cell ${cell}.`;
                } else {
                    return `Move to cell ${cell} and target ${piece}.`;
                }

            case SelectionKind.Subject:
                return `Pick up ${piece}.`;

            case SelectionKind.Target:
                return `Target ${piece} at cell ${cell}.`;

            case SelectionKind.Vacate:
                return `Vacate ${this.centerCellName()} to cell ${cell}.`;

            default: throw "Invalid selection kind.";
        }
    }

    private static getPlayerName(playerId : number, game : Game) : string {
        return game.players.find(p => p.id === playerId).name;
    }

    public static getEffectDescription(effect : Effect, game : Game, board: Board) : string {
        switch (effect.kind) {
            case EffectKind.PieceAbandoned: {
                const f = <PieceAbandonedEffect>effect.value;
                return `${Copy.getPieceLabel(f.oldPiece, game)} was abandoned.`;
            }
            case EffectKind.PieceDropped: {
                const f = <PieceDroppedEffect>effect.value;
                //Get newpiece name here because the piece may have transitioned into corpsehood
                return `${Copy.getPieceLabel(f.newPiece, game)} was dropped at ${Copy.getCellLabel(f.newPiece.cellId, board)}.`;
            }
            case EffectKind.PieceEnlisted: {
                const f = <PieceEnlistedEffect>effect.value;
                return `${Copy.getPieceLabel(f.oldPiece, game)} was enlisted by ${Copy.getPlayerName(f.newPlayerId, game)}.`;
            }
            case EffectKind.PieceKilled: {
                const f = <PieceKilledEffect>effect.value;
                return `${Copy.getPieceLabel(f.oldPiece, game)} was killed.`;
            }
            case EffectKind.PieceMoved: {
                const f = <PieceMovedEffect>effect.value;
                return `${Copy.getPieceLabel(f.oldPiece, game)} was moved to ${Copy.getCellLabel(f.newCellId, board)}.`;
            }
            case EffectKind.PieceVacated: {
                const f = <PieceVacatedEffect>effect.value;
                return `${Copy.getPieceLabel(f.oldPiece, game)} vacated the ${this.centerCellName()} to ${Copy.getCellLabel(f.newCellId, board)}.`;
            }
            case EffectKind.PlayerOutOfMoves: {
                const f = <PlayerOutOfMovesEffect>effect.value;
                return `${Copy.getPlayerName(f.playerId, game)} is out of moves.`;
            }
            case EffectKind.PlayerStatusChanged: {
                const f = <PlayerStatusChangedEffect>effect.value;
                switch (f.newStatus) {
                    case PlayerStatus.Eliminated:
                        return `${Copy.getPlayerName(f.playerId, game)} was eliminated.`;
                    case PlayerStatus.Victorious:
                        return `${Copy.getPlayerName(f.playerId, game)} won.`;
                    case PlayerStatus.Alive:
                        return `${Copy.getPlayerName(f.playerId, game)} will no longer accept a draw.`;
                    case PlayerStatus.WillConcede:
                        return `${Copy.getPlayerName(f.playerId, game)} will concede at the start of their next turn.`;
                    case PlayerStatus.Conceded:
                        return `${Copy.getPlayerName(f.playerId, game)} conceded.`;
                    case PlayerStatus.AcceptsDraw:
                        return `${Copy.getPlayerName(f.playerId, game)} will accept a draw.`;
                    default:
                        throw "Unsupported player status: " + f.newStatus;
                }
            }
            case EffectKind.TurnCyclePlayerFellFromPower: {
                const f = <TurnCyclePlayerFellFromPowerEffect>effect.value;
                return `${Copy.getPlayerName(f.playerId, game)} fell from power.`;
            }
            case EffectKind.TurnCyclePlayerRoseToPower: {
                const f = <TurnCyclePlayerRoseToPowerEffect>effect.value;
                return `${Copy.getPlayerName(f.playerId, game)} rose to power.`;
            }
            default:
                throw "Unsupported effect kind: " + effect.kind;
        }
    }

    public static getEventDescription(event : Event, game : Game) : string {
        const agent = Copy.getAgentName(event, game);

        switch (event.kind) {
            case EventKind.GameStarted:
                return `Game started`;

            case EventKind.TurnCommitted:
                return `${agent} took a turn`;

            case EventKind.PlayerStatusChanged:
                const f = event.effects.find(x => x.kind === EffectKind.PlayerStatusChanged);
                const s = (<PlayerStatusChangedEffect>f.value).newStatus;
                switch (s) {
                    case PlayerStatus.AcceptsDraw:
                        return `${agent} will accept a draw`;
                    case PlayerStatus.Conceded:
                        return `${agent} conceded`
                    case PlayerStatus.Alive:
                        return `${agent} will no longer accept a draw`;
                    default:
                        throw "Unsupported player status: " + s;
                }

            default:
                throw "Unsupported event kind: " + event.kind;
        }
    }

    private static getAgentName(e : Event, g : Game): string {
        if (e.actingPlayerId) {
            return g.players.find(p => p.id === e.actingPlayerId).name;
        } else if (e.createdBy.userId) {
            return e.createdBy.userName;
        } else {
            return "System";
        }
    }
}