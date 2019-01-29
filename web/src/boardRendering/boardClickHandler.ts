import BoardRenderer from "./boardRenderer";
import ApiClient from "../api/client";
import { BoardView } from "./model";
import BoardGeometry from "./boardGeometry";

export class BoardClickHandler {
    constructor(
        readonly renderer : BoardRenderer,
        readonly canvas : HTMLCanvasElement,
        readonly board : BoardView,
        readonly api : ApiClient,
        readonly gameId : number
    ){
    }

    async clickOnCell(gameId : number, cellId : number) {
        const response = await this.api.selectCell(gameId, cellId);
        this.renderer.updateTurn(response.game.currentTurn);
    }

    async clickOnBoard(e : MouseEvent) {
        const point = {
            x: e.pageX - this.canvas.offsetLeft,
            y: e.pageY - this.canvas.offsetTop
        };

        const cell = BoardGeometry.boardCellAtPoint(this.board, point);
        if (cell) {
            this.clickOnCell(this.gameId, cell.id);
        }
    }
}