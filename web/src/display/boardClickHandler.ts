import BoardRenderer from "./boardRenderer";
import BoardView from "./boardView";
import Point from "../geometry/point";
import ApiClient from "../api/client";

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
        const point = new Point(
            e.pageX - this.canvas.offsetLeft,
            e.pageY - this.canvas.offsetTop
        );

        const cell = this.board.cellAtPoint(point);
        if (cell) {
            this.clickOnCell(this.gameId, cell.id);
        }
    }
}