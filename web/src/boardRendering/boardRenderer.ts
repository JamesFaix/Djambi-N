import { Game } from "../api/model";
import ThemeService from "../themes/themeService";
import Geometry from "../geometry/geometry";
import { BoardView } from "./model";
import BoardGeometry from "./boardGeometry";
import { Point } from "../geometry/model";

export default class BoardRenderer {
    private lastGameState : Game;
    constructor(
        readonly offset : Point,
        readonly boardView : BoardView,
        readonly theme : ThemeService){
    }

    onPieceClicked : (gameId : number, cellId : number) => any;

    refresh(){
        this.updateGame(this.lastGameState);
    }

    updateGame(game : Game){
        this.lastGameState = game;
        this.drawPieces(game);
    }

    clearPieces() : void {
        document.querySelectorAll("[id^='div_board_piece']")
            .forEach(div => div.parentElement.removeChild(div));
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

        let offset = { x: -size/2, y: -size/2 };
        offset = Geometry.pointTranslate(offset, this.offset);

        const playerColorsHighlightStyles = new Map();
        for(var i = 0; i < game.players.length; i++) {
            const player = game.players[i];
            const hexColor = this.theme.getPlayerColor(player.colorId);
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
            let centroid = Geometry.pointTranslate(BoardGeometry.cellCentroid(cell), offset);

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
            innerDiv.innerHTML = this.theme.getPieceImage(piece.kind);

            div.appendChild(innerDiv);
            boardDiv.appendChild(div);
        }
    }
}