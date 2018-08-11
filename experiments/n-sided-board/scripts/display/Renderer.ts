import {VisualCell} from "./VisualCell.js";
import {VisualBoard} from "./VisualBoard.js";
import {CellState} from "./CellState.js";
import {Color} from "./Color.js";

export class Renderer {
    constructor(){

    }

    static drawCell(cell : VisualCell, canvas : HTMLCanvasElement) : void {
        var ctx = canvas.getContext("2d");

        switch (cell.state)
        {
            case CellState.Default:
                ctx.fillStyle = cell.color.toHex();
                break;

            case CellState.Selected:
                ctx.fillStyle = cell.color.lighten(0.75)
                                          .multiply(Color.greenHighlight())
                                          .toHex();
                break;

            case CellState.Selectable:
                ctx.fillStyle = cell.color.lighten(0.50)
                                          .multiply(Color.yellowHighlight())
                                          .toHex();
                break;
        }

        cell.polygons.forEach(p => {
            ctx.beginPath();
        
            var v = p.vertices[0];
            ctx.moveTo(v.x, v.y);
        
            for (var i = 1; i < p.vertices.length; i++) {
                v = p.vertices[i];
                ctx.lineTo(v.x, v.y);
            }
        
            ctx.closePath();    
            ctx.fill();
        })
    }

    static drawBoard(board: VisualBoard, canvas : HTMLCanvasElement) : void {
        board.cells.forEach(c => Renderer.drawCell(c, canvas));
    }
}