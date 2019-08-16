import { Board, Game } from '../../api/model';
import { BoardView, CellState } from './model';
import BoardViewFactory from './boardViewFactory';

export default class BoardViewService {
    private readonly emptyBoardViewCache : any = {};

    create(board : Board, game : Game) : BoardView {
        const empty = this.getEmptyBoardView(board);
        return BoardViewService.updateBoardView(empty, game);
    }

    private getEmptyBoardView(board : Board) : BoardView {
        //The empty state of a board with the same dimensions is always the same, so it can be cached.
        const key = board.regionCount + "-" + board.regionSize;
        let bv = this.emptyBoardViewCache[key];
        if (!bv) {
            bv = BoardViewFactory.createEmptyBoardView(board);
        }
        return bv;
    }

    private static updateBoardView(board : BoardView, game : Game) : BoardView {

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
            const pieceView = piece ? { id : piece.id, kind: piece.kind, colorId: colorId } : null;

            return {
                id: c.id,
                locations: c.locations,
                type: c.type,
                state: newState,
                piece: pieceView,
                polygon: c.polygon
            };
        });

        return {
            regionCount : board.regionCount,
            cellCountPerSide: board.cellCountPerSide,
            cells : newCells,
            polygon : board.polygon
        };
    }
}