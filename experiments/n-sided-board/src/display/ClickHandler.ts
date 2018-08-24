import {Point} from "../geometry/Point";
import {VisualBoard} from "./VisualBoard";
import {PlayClient} from "../apiClient/PlayClient";
import {Renderer} from "./Renderer";

export class ClickHandler {
    constructor(
        readonly renderer : Renderer,
        readonly canvas : HTMLCanvasElement,
        readonly board : VisualBoard
    ){
    }

    async clickOnBoard(e : MouseEvent) {        
        const point = new Point(
            e.pageX - this.canvas.offsetLeft,
            e.pageY - this.canvas.offsetTop
        );
    
        const cell = this.board.cellAtPoint(point);
        if (cell) {
            const turnState = await PlayClient.selectCell(this.board.gameId, cell.id);
            this.renderer.updateTurn(turnState);
        }
    }

    async clickOnCell(gameId : number, cellId : number) {
        const turnState = await PlayClient.selectCell(gameId, cellId);
        this.renderer.updateTurn(turnState);
    }
}