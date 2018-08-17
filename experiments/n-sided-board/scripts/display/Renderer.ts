import {VisualCell} from "./VisualCell.js";
import {VisualBoard} from "./VisualBoard.js";
import {CellState} from "./CellState.js";
import {Color} from "./Color.js";
import { Piece, PieceType, GameState } from "../apiClient/PlayModel.js";
import { Point } from "../geometry/Point.js";

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

    static drawPieces(board : VisualBoard, canvas : HTMLCanvasElement, gameState : GameState) : void {
        let boardDiv = document.getElementById("div_board");

        //Remove old piece divs
        let children = boardDiv.children;
        for (var i = 0; i < children.length; i++){
            let ch = children.item(i);
            if (ch.id.startsWith("div_" + canvas.id + "_piece")){
                boardDiv.removeChild(ch)
            }
        }

        let size = board.cellSize / 2;
        let fontSize = 36;
        let pieceStyle = "position: absolute; z-index: 1; " + 
            "width: " + size + "px; height: " + size + "px; " +
            "text-align: center; " +
            "font-size: " + fontSize + "px; "

        let offset = new Point(-fontSize/2, -fontSize/2);
        offset = offset.translate(new Point(canvas.offsetLeft, canvas.offsetTop));

        //Add new piece divs
        for (var i = 0; i < gameState.pieces.length; i++){
            let piece = gameState.pieces[i];

            let cell = board.cells.find(c => c.id === piece.cellId);
            let centroid = cell.centroid().translate(offset);

            let div = document.createElement("div");
            div.id = "div_" + canvas.id + "_piece" + piece.id
            div.setAttribute("style", pieceStyle + 
                "left: " + centroid.x + "px; top: " + centroid.y + "px");

            div.innerHTML = this.getPieceEmoji(piece);

            boardDiv.appendChild(div);
        }
    }

    private static getPieceEmoji(piece : Piece) : string {
        if (!piece.isAlive){
            return "&#x1F480";
        }

        switch (piece.type) {
            case PieceType.Chief : return "&#x1F451"
            case PieceType.Assassin : return "&#x1F5E1"
            case PieceType.Diplomat : return "&#x1F54A"
            case PieceType.Reporter : return "&#x1F4F0"
            case PieceType.Gravedigger : return "&#x26B0"
            case PieceType.Thug : return "&#x270A"
        }

        throw "Invalid piece type '" +  piece.type + "'";
    }
}