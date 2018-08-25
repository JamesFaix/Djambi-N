import { VisualBoard } from "./VisualBoard";
import {TurnState, GameState, PlayerStartConditions, SelectionType} from "../apiClient/PlayModel";
import {CellState} from "./CellState";
import {VisualCell} from "./VisualCell";
import {Color} from "./Color";
import {Point} from "../geometry/Point";
import {ITheme} from "./Theme";

export class Renderer {
    private lastGameState : GameState;
    private lastTurnState : TurnState;
    theme : ITheme;
    constructor(
        readonly canvas : HTMLCanvasElement,
        readonly board : VisualBoard,
        readonly startingConditions : Array<PlayerStartConditions>){
    }
    
    onPieceClicked : (gameId : number, cellId : number) => any;

    refresh(){
        this.updateGame(this.lastGameState, this.lastTurnState);
    }

    updateTurn(turnState : TurnState) {
        this.lastTurnState = turnState;

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

        this.updateRequiredSelectionPrompt(turnState.requiredSelectionType);
        this.updateTurnDescription(this.lastGameState, turnState);
        this.enableOrDisableTurnButtons(turnState);
    }

    updateGame(gameState : GameState, turnState : TurnState){
        this.lastGameState = gameState;

        this.updateTurn(turnState);
        this.drawPieces(gameState);
    }

    clearPieces() : void {
        document.querySelectorAll("[id^='div_board_piece']")
            .forEach(div => div.parentElement.removeChild(div));
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
        this.clearPieces();
        let boardDiv = document.getElementById("div_board");
        let size = 2;
        let fontsize = 36;

        let pieceStyle = "position: absolute; z-index: 1; " + 
            "width: " + size + "px; height: " + size + "px; " +
            "text-align: center; " +
            "font-size: " + fontsize + "px; " + 
            "font-family: 'Segoe UI Emoji'; " +
            "border: 1px solid red; "

        const emojiStyle = "position: absolute; top: -24px; left: -24px;";

        let offset = new Point(-size/2, -size/2);
        offset = offset.translate(new Point(this.canvas.offsetLeft, this.canvas.offsetTop));

        const playerColorsHighlightStyles = new Map();
        for(var i = 0; i < this.startingConditions.length; i++) {
            const player = this.startingConditions[i];
            const hexColor = this.theme.getPlayerColor(player.color);
            const style = "text-shadow: 0px 0px 20px " + hexColor + 
                                     ", 0px 0px 20px " + hexColor + 
                                     ", 0px 0px 20px " + hexColor + ";"; //triple shadow to make it wide and dark
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
            div.id = "div_board_piece" + piece.id
            div.setAttribute("style", pieceStyle + 
                "left: " + centroid.x + "px; top: " + centroid.y + "px; " +
                playerColorsHighlightStyles.get(piece.playerId));

            if (_this.onPieceClicked) {
                div.onclick = () => _this.onPieceClicked(_this.board.gameId, cell.id);
            }

            let innerDiv = document.createElement("div");
            innerDiv.setAttribute("style", emojiStyle);
            innerDiv.innerHTML = this.theme.getPieceEmoji(piece);      
            
            div.appendChild(innerDiv);
            boardDiv.appendChild(div);
        }
    }

    private updateRequiredSelectionPrompt(requiredSelectionType : SelectionType) : void {
        const div = document.getElementById("div_turnPrompt");
        const prompt = this.theme.getRequiredSelectionPrompt(requiredSelectionType);
        div.innerHTML = prompt;
    }

    private updateTurnDescription(gameState: GameState, turnState: TurnState) : void {
        const div = document.getElementById("div_turnDescription");

        const text = turnState.selections
            .map(s => this.theme.getSelectionDescription(s, gameState, this.board))
            .join("");

        div.innerHTML = text;
    }

    private enableOrDisableTurnButtons(turnState : TurnState) : void {
        const commitButton = document.getElementById("btn_turnConfirm");
        const resetButton = document.getElementById("btn_turnReset");

        if (turnState.requiredSelectionType === null) {
            commitButton.removeAttribute("disabled");
        }
        else {
            commitButton.setAttribute("disabled", null);
        }

        if (turnState.selections.length > 0) {
            resetButton.removeAttribute("disabled");
        }
        else {
            resetButton.setAttribute("disabled", null);
        }
    }
}