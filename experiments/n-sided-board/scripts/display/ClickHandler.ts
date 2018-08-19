import {Point} from "../geometry/Point.js";
import {VisualBoard} from "./VisualBoard.js";
import {Renderer} from "./Renderer.js";
import {CellState} from "./CellState.js";
import { VisualCell } from "./VisualCell.js";
import {BoardClient} from "../apiClient/BoardClient.js";
import { Piece } from "../apiClient/PlayModel.js";

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
        await ClickHandler.highlightPaths(canvas, board, cell);
    }

    static async clickOnPiece(
        piece : Piece,
        board : VisualBoard, 
        canvas : HTMLCanvasElement) {    

        const cell = board.cellById(piece.cellId);    
        await ClickHandler.highlightPaths(canvas, board, cell);
    }
        
    private static async highlightPaths(
        canvas : HTMLCanvasElement, 
        board : VisualBoard, 
        cell : VisualCell) {
    
        board.cells.forEach(c => c.state = CellState.Default);
    
        if (cell) {
            cell.state = CellState.Selected;
            let paths = await BoardClient.getCellPaths(board.regionCount, cell.id);
            paths.map(path => path.map(id => board.cells.find(c => c.id === id)))
                .reduce((p1, p2) => p1.concat(p2))
                .forEach(c => c.state = CellState.Selectable);
        } 
    
        Renderer.drawBoard(board, canvas);
    }
}