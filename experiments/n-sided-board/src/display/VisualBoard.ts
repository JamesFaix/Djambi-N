import {Point} from "../geometry/Point";
import {VisualCell} from "./VisualCell";

export class VisualBoard {
    constructor (
        readonly regionCount : number,
        readonly cellSize : number,
        readonly cells : Array<VisualCell>,
        readonly gameId : number
    ) {
    }

    cellAtPoint(point: Point) : VisualCell {
        return this.cells.find(vc => vc.contains(point));
    }

    cellById(id : number) : VisualCell {
        return this.cells.find(vc => vc.id === id);
    }
}