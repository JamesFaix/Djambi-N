import { VisualBoard } from "./VisualBoard.js";
import {TurnState, GameState, PieceType, Piece, PlayerStartConditions} from "../apiClient/PlayModel.js";
import {CellState} from "./CellState.js";
import {VisualCell} from "./VisualCell.js";
import {Color} from "./Color.js";
import {Point} from "../geometry/Point.js";

export class Renderer {
    constructor(
        readonly canvas : HTMLCanvasElement,
        readonly board : VisualBoard,
        readonly startingConditions : Array<PlayerStartConditions>){

    }
    
    onPieceClicked : (gameId : number, cellId : number) => any;

    updateTurn(turnState : TurnState) {
        this.board.cells.forEach(c => c.state = CellState.Default);

        turnState.selectionOptions
            .map(id => this.board.cells.find(c => c.id === id))
            .forEach(c => c.state = CellState.Selectable);

        turnState.selections
            .map(s => s.cellId)
            .filter(id => id !== null)
            .map(id => this.board.cells.find(c => c.id === id))
            .forEach(c => c.state = CellState.Selected)

        this.board.cells.forEach(c => this.drawCell(c));
    }

    updateGame(gameState : GameState, turnState : TurnState){
        this.updateTurn(turnState);
        this.drawPieces(gameState);
    }

    private drawCell(cell : VisualCell) : void {
        var ctx = this.canvas.getContext("2d");

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

    private drawPieces(gameState : GameState) {
        let boardDiv = document.getElementById("div_board");

        //Remove old piece divs
        let children = boardDiv.children;
        for (var i = 0; i < children.length; i++){
            let ch = children.item(i);
            if (ch.id.startsWith("div_" + this.canvas.id + "_piece")){
                boardDiv.removeChild(ch)
            }
        }

        let size = this.board.cellSize / 2;
        let fontSize = 36;
        let pieceStyle = "position: absolute; z-index: 1; " + 
            "width: " + size + "px; height: " + size + "px; " +
            "text-align: center; " +
            "font-size: " + fontSize + "px; " + 
            "font-family: 'Segoe UI Emoji';"

        let offset = new Point(-fontSize/2, -fontSize/2);
        offset = offset.translate(new Point(this.canvas.offsetLeft, this.canvas.offsetTop));

        const playerColorsHighlightStyles = new Map();
        for(var i = 0; i < this.startingConditions.length; i++) {
            const player = this.startingConditions[i];
            const hexColor = this.getPlayerColor(player.color);
            const style = "text-shadow: 0px 0px 20px " + hexColor + ", 0px 0px 20px " + hexColor + ", 0px 0px 20px " + hexColor + ";"; //triple shadow to make it wide and dark
            playerColorsHighlightStyles.set(player.playerId, style);
        }

        //Must put in local because event handler's `this` will be the HTML element
        const _this = this;

        //Add new piece divs
        for (var i = 0; i <  gameState.pieces.length; i++){
            let piece =  gameState.pieces[i];
            
            let cell = this.board.cells.find(c => c.id === piece.cellId);
            let centroid = cell.centroid().translate(offset);

            let div = document.createElement("div");
            div.id = "div_" + this.canvas.id + "_piece" + piece.id
            div.setAttribute("style", pieceStyle + 
                "left: " + centroid.x + "px; top: " + centroid.y + "px; " +
                playerColorsHighlightStyles.get(piece.playerId));

            div.innerHTML = this.getPieceEmoji(piece);      
            
            if (_this.onPieceClicked) {
                div.onclick = () => _this.onPieceClicked(_this.board.gameId, cell.id);
            }
            boardDiv.appendChild(div);
        }
    }

    private getPieceEmoji(piece : Piece) : string {
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

    private getPlayerColor(colorId : number) : string {
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