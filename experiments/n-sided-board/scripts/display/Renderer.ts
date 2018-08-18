import {VisualCell} from "./VisualCell.js";
import {VisualBoard} from "./VisualBoard.js";
import {CellState} from "./CellState.js";
import {Color} from "./Color.js";
import { Piece, PieceType, GameState, GameStartResponse } from "../apiClient/PlayModel.js";
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

    static drawPieces(board : VisualBoard, canvas : HTMLCanvasElement, startResponse : GameStartResponse) : void {
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
            "font-size: " + fontSize + "px; " + 
            "font-family: 'Segoe UI Emoji';"

        let offset = new Point(-fontSize/2, -fontSize/2);
        offset = offset.translate(new Point(canvas.offsetLeft, canvas.offsetTop));

        const playerColorsHighlightStyles = new Map();
        for(var i = 0; i < startResponse.startingConditions.length; i++) {
            const player = startResponse.startingConditions[i];
            const hexColor = this.getPlayerColor(player.color);
            const style = "text-shadow: 0px 0px 20px " + hexColor + ", 0px 0px 20px " + hexColor + ", 0px 0px 20px " + hexColor + ";"; //triple shadow to make it wide and dark
            playerColorsHighlightStyles.set(player.playerId, style);
        }

        //Add new piece divs
        const pieces = startResponse.currentState.pieces;
        for (var i = 0; i < pieces.length; i++){
            let piece = pieces[i];
            
            let cell = board.cells.find(c => c.id === piece.cellId);
            let centroid = cell.centroid().translate(offset);

            let div = document.createElement("div");
            div.id = "div_" + canvas.id + "_piece" + piece.id
            div.setAttribute("style", pieceStyle + 
                "left: " + centroid.x + "px; top: " + centroid.y + "px; " +
                playerColorsHighlightStyles.get(piece.playerId));

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
            case PieceType.Gravedigger : return "&#x26CF"
            case PieceType.Thug : return "&#x270A"
            default: throw "Invalid piece type '" +  piece.type + "'";
        }
    }

    private static getPlayerColor(colorId : number) : string {
        switch (colorId){
            case 0: return "#CC2B08"; //Red
            case 1: return "#47CC08"; //Green
            case 2: return "#08A9CC"; //Blue
            case 3: return "#8D08CC"; //Purple
            case 4: return "#CC8708"; //Orange
            case 5: return "#CC0884"; //Pink
            case 6: return "#08CC8B"; //Teal
            case 7: return "#996A0C"; //Brown
            default: throw "Invalid player colorId: " + colorId;
        }
    }
}