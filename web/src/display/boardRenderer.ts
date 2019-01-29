import CellView from "./cellView";
import { CellState } from "./cellState";
import Color from "./color";
import { Game, Turn } from "../api/model";
import Point from "../geometry/point";
import BoardView from "./boardView";
import ThemeService from "../themes/themeService";

export default class BoardRenderer {
    private lastGameState : Game;
    constructor(
        readonly canvas : HTMLCanvasElement,
        readonly boardView : BoardView,
        readonly themeService : ThemeService){
    }

    onPieceClicked : (gameId : number, cellId : number) => any;

    refresh(){
        this.updateGame(this.lastGameState);
    }

    updateTurn(turn : Turn) {
        this.lastGameState.currentTurn = turn;

        this.boardView.cells.forEach(c => c.state = CellState.Default);

        turn.selectionOptions
            .map(id => this.boardView.cells.find(c => c.id === id))
            .forEach(c => c.state = CellState.Selectable);

        turn.selections
            .map(s => s.cellId)
            .filter(id => id !== null)
            .map(id => this.boardView.cells.find(c => c.id === id))
            .forEach(c => c.state = CellState.Selected)

        this.boardView.cells.forEach(c => this.drawCell(c));
    }

    updateGame(game : Game){
        this.lastGameState = game;

        this.updateTurn(game.currentTurn);
        this.drawPieces(game);
    }

    clearPieces() : void {
        document.querySelectorAll("[id^='div_board_piece']")
            .forEach(div => div.parentElement.removeChild(div));
    }

    private drawCell(cell : CellView) : void {
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

    private drawPieces(game : Game) : void {
        console.log("DrawPieces");
        console.log(game);

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
        for(var i = 0; i < game.players.length; i++) {
            const player = game.players[i];
            const hexColor = this.themeService.getPlayerColor(player.colorId);
            const style = "text-shadow: 0px 0px 20px " + hexColor +
                                     ", 0px 0px 20px " + hexColor +
                                     ", 0px 0px 20px " + hexColor + ";"; //triple shadow to make it wide and dark
            playerColorsHighlightStyles.set(player.id, style);
        }

        //Must put in local because event handler's `this` will be the HTML element
        const _this = this;

        //Add new piece divs
        for (var i = 0; i <  game.pieces.length; i++){
            let piece =  game.pieces[i];

            let cell = this.boardView.cells.find(c => c.id === piece.cellId);
            let centroid = cell.centroid().translate(offset);

            let div = document.createElement("div");
            div.id = "div_board_piece" + piece.id
            div.setAttribute("style", pieceStyle +
                "left: " + centroid.x + "px; top: " + centroid.y + "px; " +
                playerColorsHighlightStyles.get(piece.playerId));

            if (_this.onPieceClicked) {
                div.onclick = () => _this.onPieceClicked(game.id, cell.id);
            }

            let innerDiv = document.createElement("div");
            innerDiv.setAttribute("style", emojiStyle);
            innerDiv.innerHTML = this.themeService.getPieceEmoji(piece.kind);

            div.appendChild(innerDiv);
            boardDiv.appendChild(div);
        }
    }
}