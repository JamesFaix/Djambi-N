import { Turn, Selection, Game, TurnStatus, SelectionKind, Location, Piece, PieceKind, Board, Effect, Event, EventKind, EffectKind, PlayerStatusChangedEffect, PlayerStatus, PieceAbandonedEffect, PieceDroppedEffect, PieceEnlistedEffect, PieceKilledEffect, PieceMovedEffect, PieceVacatedEffect, PlayerOutOfMovesEffect, TurnCyclePlayerFellFromPowerEffect, TurnCyclePlayerRoseToPowerEffect } from "../api/model";
import Debug from "../debug";
import { BoardView } from "../viewModel/board/model";
import ThemeService from "../themes/themeService";

export function boolToYesOrNo(value : boolean) : string {
    if (value === true) {
        return "Yes";
    } else if (value === false) {
        return "No"
    } else { //null/undefined
        return null;
    }
}

function locationToString(location : Location) : string {
    return `(${location.region}, ${location.x}, ${location.y})`;
}

export function getCellLabel(theme : Theme, cellId : number, board : Board) : string {
    //The first time the board is needed it must be fetched from the API.
    //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
    if (board) {
        const cell = board.cells.find(c => c.id === cellId);

        const base = cell.locations.find(l => l.x === 0 && l.y === 0)
            ? theme.copy.centerCellName
            : locationToString(cell.locations[0]);

        if (Debug.showPieceAndCellIds){
            return `${base} (${cellId})`;
        } else {
            return base;
        }
    } else {
        return cellId.toString();
    }
}

export function getCellViewLabel(theme : Theme, cellId : number, board : BoardView) : string {
    //The first time the board is needed it must be fetched from the API.
    //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
    if (board) {
        const cell = board.cells.find(c => c.id === cellId);

        const base = cell.locations.find(l => l.x === 0 && l.y === 0)
            ? theme.copy.centerCellName
            : locationToString(cell.locations[0]);

        if (Debug.showPieceAndCellIds){
            return `${base} (${cellId})`;
        } else {
            return base;
        }
    } else {
        return cellId.toString();
    }
}

function getPieceLabel(theme : Theme, piece : Piece, game : Game) : string {
    const kindName = ThemeService.getPieceName(theme, piece.kind);

    if (piece.kind === PieceKind.Corpse) {
        return kindName;
    }

    const player = game.players.find(p => p.id === piece.playerId);
    const base = player
        ? `${player.name}'s ${kindName}`
        : `Neutral ${kindName}`;

    if (Debug.showPieceAndCellIds){
        return `${base} (#${piece.id})`;
    } else {
        return base;
    }
}

export function getTurnPrompt(turn : Turn) : string {
    switch (turn.status) {
        case TurnStatus.AwaitingSelection:
            return getSelectionPrompt(turn.requiredSelectionKind);
        case TurnStatus.AwaitingCommit:
            return "End your turn or reset.";
        case TurnStatus.DeadEnd:
            return "No futher selections are available. You must reset.";
        default:
            throw "Invalid turn status.";
    }
}

function getSelectionPrompt(kind : SelectionKind) : string {
    switch (kind) {
        case SelectionKind.Drop: return "Select a cell to drop the target piece in.";
        case SelectionKind.Move: return "Select a cell to move to.";
        case SelectionKind.Subject: return "Select a piece to move.";
        case SelectionKind.Target: return "Select a piece to target.";
        case SelectionKind.Vacate: return "Select a cell to vacate to.";
        default: throw "Invalid selection kind.";
    }
}

export function getSelectionDescription(theme : Theme, selection : Selection, game : Game, board : Board) : string {
    const cell = selection.cellId
        ? getCellLabel(theme, selection.cellId, board)
        : null;

    const piece = selection.pieceId
        ? getPieceLabel(theme, game.pieces.find(p => p.id === selection.pieceId), game)
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
            return `Vacate ${theme.copy.centerCellName} to cell ${cell}.`;

        default: throw "Invalid selection kind.";
    }

}

function getPlayerName(playerId : number, game : Game) : string {
    return game.players.find(p => p.id === playerId).name;
}

export function getEffectDescription(theme : Theme, effect : Effect, game : Game, board: Board) : string {
    switch (effect.kind) {
        case EffectKind.PieceAbandoned: {
            const f = <PieceAbandonedEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} was abandoned.`;
        }
        case EffectKind.PieceDropped: {
            const f = <PieceDroppedEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} was dropped at ${getCellLabel(theme, f.newCellId, board)}.`;
        }
        case EffectKind.PieceEnlisted: {
            const f = <PieceEnlistedEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} was enlisted by ${getPlayerName(f.newPlayerId, game)}.`;
        }
        case EffectKind.PieceKilled: {
            const f = <PieceKilledEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} was killed.`;
        }
        case EffectKind.PieceMoved: {
            const f = <PieceMovedEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} was moved to ${getCellLabel(theme, f.newCellId, board)}.`;
        }
        case EffectKind.PieceVacated: {
            const f = <PieceVacatedEffect>effect.value;
            return `${getPieceLabel(theme, f.oldPiece, game)} vacated the ${theme.copy.centerCellName} to ${getCellLabel(theme, f.newCellId, board)}.`;
        }
        case EffectKind.PlayerOutOfMoves: {
            const f = <PlayerOutOfMovesEffect>effect.value;
            return `${getPlayerName(f.playerId, game)} is out of moves.`;
        }
        case EffectKind.PlayerStatusChanged: {
            const f = <PlayerStatusChangedEffect>effect.value;
            switch (f.newStatus) {
                case PlayerStatus.Eliminated:
                    return `${getPlayerName(f.playerId, game)} was eliminated.`;
                case PlayerStatus.Victorious:
                    return `${getPlayerName(f.playerId, game)} won.`;
                case PlayerStatus.Alive:
                    return `${getPlayerName(f.playerId, game)} will no longer accept a draw.`;
                case PlayerStatus.WillConcede:
                    return `${getPlayerName(f.playerId, game)} will concede at the start of their next turn.`;
                case PlayerStatus.Conceded:
                    return `${getPlayerName(f.playerId, game)} conceded.`;
                case PlayerStatus.AcceptsDraw:
                    return `${getPlayerName(f.playerId, game)} will accept a draw.`;
                default:
                    throw "Unsupported player status: " + f.newStatus;
            }
        }
        case EffectKind.TurnCyclePlayerFellFromPower: {
            const f = <TurnCyclePlayerFellFromPowerEffect>effect.value;
            return `${getPlayerName(f.playerId, game)} fell from power.`;
        }
        case EffectKind.TurnCyclePlayerRoseToPower: {
            const f = <TurnCyclePlayerRoseToPowerEffect>effect.value;
            return `${getPlayerName(f.playerId, game)} rose to power.`;
        }
        default:
            throw "Unsupported effect kind: " + effect.kind;
    }
}

export function getEventDescription(event : Event, game : Game) : string {
    const agent = getAgentName(event, game);

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

function getAgentName(e : Event, g : Game): string {
    if (e.actingPlayerId) {
        return g.players.find(p => p.id === e.actingPlayerId).name;
    } else if (e.createdBy.userId) {
        return e.createdBy.userName;
    } else {
        return "System";
    }
}