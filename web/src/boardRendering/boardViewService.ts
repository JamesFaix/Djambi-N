import { BoardView, CellState } from "./model";
import { Game } from "../api/model";

export default class BoardViewService {

    public static update(board : BoardView, game : Game) : BoardView {

        const newCells = board.cells.map(c => {
            let newState = CellState.Default;
            const turn = game.currentTurn;

            if (turn) {
                if (turn.selections.find(s => s.cellId === c.id)) {
                    newState = CellState.Selected;
                } else if (turn.selectionOptions.find(cellId => cellId === c.id)) {
                    newState = CellState.Selectable;
                }
            }

            const piece = game.pieces.find(p => p.cellId === c.id);
            const owner = piece ? game.players.find(p => p.id === piece.playerId) : null;
            const colorId = owner ? owner.colorId : null;
            const pieceView = piece ? { kind: piece.kind, colorId: colorId } : null;

            return {
                id: c.id,
                type: c.type,
                state: newState,
                piece: pieceView,
                polygons: c.polygons
            }
        });

        return {
            regionCount : board.regionCount,
            cellSize : board.cellSize,
            cells : newCells
        }
    }
}