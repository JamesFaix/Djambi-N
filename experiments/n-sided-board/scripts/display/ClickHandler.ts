import {Point} from "../geometry/Point.js";
import {VisualBoard} from "./VisualBoard.js";
import {Renderer} from "./Renderer.js";
import {CellState} from "./CellState.js";
import { VisualCell } from "./VisualCell.js";
import { Piece } from "../apiClient/PlayModel.js";
import {PlayClient} from "../apiClient/PlayClient.js";

export class ClickHandler {

    static async clickOnBoard(
        e : MouseEvent, 
        board : VisualBoard, 
        canvas : HTMLCanvasElement) {
        
        const point = new Point(
            e.pageX - canvas.offsetLeft,
            e.pageY - canvas.offsetTop
        );
    
        const cell = board.cellAtPoint(point);
        await this.selectCellAndUpdateCellStatuses(canvas, board, cell);
    }

    static async clickOnPiece(
        piece : Piece,
        board : VisualBoard, 
        canvas : HTMLCanvasElement) {    

        const cell = board.cellById(piece.cellId);    
        await this.selectCellAndUpdateCellStatuses(canvas, board, cell);
    }
        
    private static async selectCellAndUpdateCellStatuses(
        canvas : HTMLCanvasElement, 
        board : VisualBoard, 
        cell : VisualCell) {
    
        await PlayClient.selectCell(board.gameId, cell.id)
            .then(response => {
                board.cells.forEach(c => c.state = CellState.Default);

                response.selectionOptions
                    .map(id => board.cells.find(c => c.id === id))
                    .forEach(c => c.state = CellState.Selectable);
    
                response.selections
                    .map(s => s.cellId)
                    .filter(id => id !== null)
                    .map(id => board.cells.find(c => c.id === id))
                    .forEach(c => c.state = CellState.Selected)
    
                Renderer.drawBoard(board, canvas);
            });
    }
}