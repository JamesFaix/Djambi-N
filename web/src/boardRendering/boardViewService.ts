import { BoardView, CellState } from "./model";
import { Turn } from "../api/model";

export default class BoardViewService {

    public static update(board : BoardView, currentTurn : Turn) : BoardView {

        const newCells = board.cells.map(c => {
            const oldState = c.state;
            let newState = CellState.Default;

            if (currentTurn.selections.find(s => s.cellId === c.id)) {
                newState = CellState.Selected;
            } else if (currentTurn.selectionOptions.find(cellId => cellId === c.id)) {
                newState = CellState.Selectable;
            }

            if (oldState === newState) {
                return c;
            } else {
                return {
                    id: c.id,
                    type: c.type,
                    state: newState,
                    polygons: c.polygons
                };
            }
        });

        return {
            regionCount : board.regionCount,
            cellSize : board.cellSize,
            cells : newCells
        }
    }
}