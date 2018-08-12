import {Point} from "../geometry/Point.js";
import {VisualBoard} from "./VisualBoard.js";
import {Renderer} from "./Renderer.js";
import {CellState} from "./CellState.js";
import { VisualCell } from "./VisualCell.js";

export class ClickHandler {

    static async logClickOnBoard(
        e : MouseEvent, 
        board : VisualBoard, 
        canvas : HTMLCanvasElement) {
        
        const point = new Point(
            e.pageX - canvas.offsetLeft,
            e.pageY - canvas.offsetTop
        );
    
        const cell = board.cellAtLocation(point);
    
        if (cell) {
            console.log("Clicked on Board" + board.regionCount + " " + point.toString() + " " + cell.toString());
        }
    
        await ClickHandler.highlightPaths(canvas, board, cell);
    }
    
    private static highlightNeighbors(
        canvas : HTMLCanvasElement, 
        board : VisualBoard, 
        cell : VisualCell) {
    
        board.cells.forEach(c => c.state = CellState.Default);
    
        if (cell) {
            cell.state = CellState.Selected;
            board.cellNeighbors(cell).forEach(c => c.state = CellState.Selectable);
        } 
    
        Renderer.drawBoard(board, canvas);
    }
    
    
    private static async highlightPaths(
        canvas : HTMLCanvasElement, 
        board : VisualBoard, 
        cell : VisualCell) {
    
        board.cells.forEach(c => c.state = CellState.Default);
    
        if (cell) {
            cell.state = CellState.Selected;
            let selectionOptions = await board.makeSelection(cell);
            selectionOptions.forEach(c => c.state = CellState.Selectable);
        } 
    
        Renderer.drawBoard(board, canvas);
    }
}