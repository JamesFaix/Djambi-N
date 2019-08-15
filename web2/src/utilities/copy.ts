import { Turn, Selection, Game, TurnStatus, SelectionKind, Location, Piece, PieceKind, Board } from "../api/model";
import Debug from "../debug";

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

const centerCellName = "Seat";

function getCellLabel(cellId : number, board : Board) : string {
    //The first time the board is needed it must be fetched from the API.
    //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
    if (board) {
        const cell = board.cells.find(c => c.id === cellId);

        const base = cell.locations.find(l => l.x === 0 && l.y === 0)
            ? centerCellName
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

function getPieceKindName(kind : PieceKind) : string {
    switch(kind) {
        case PieceKind.Assassin: return "Assassin";
        case PieceKind.Chief: return "Chief";
        case PieceKind.Corpse: return "Corpse";
        case PieceKind.Diplomat: return "Diplomat";
        case PieceKind.Gravedigger: return "Gravedigger";
        case PieceKind.Reporter: return "Reporter";
        case PieceKind.Thug: return "Thug";
        default: throw "Invalid piece kind";
    }
}

function getPieceLabel(piece : Piece, game : Game) : string {
    const kindName = getPieceKindName(piece.kind);

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

export function getSelectionDescription(selection : Selection, game : Game, board : Board) : string {
    const cell = selection.cellId
        ? getCellLabel(selection.cellId, board)
        : null;

    const piece = selection.pieceId
        ? getPieceLabel(game.pieces.find(p => p.id === selection.pieceId), game)
        : null;

    switch (selection.kind) {
        case SelectionKind.Drop:
            return `Drop target piece at cell ${cell}`;

        case SelectionKind.Move:
            if (piece === null) {
                return `Move to cell ${cell}`;
            } else {
                return `Move to cell ${cell} and target ${piece}`;
            }

        case SelectionKind.Subject:
            return `Pick up ${piece}`;

        case SelectionKind.Target:
            return `Target ${piece} at cell ${cell}`;

        case SelectionKind.Vacate:
            return `Vacate ${centerCellName} to cell ${cell}`;

        default: throw "Invalid selection kind.";
    }

}