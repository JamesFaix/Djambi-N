import {Point} from "../geometry/Point.js";
import {VisualCell} from "./VisualCell.js";

export class VisualBoard {
    constructor (
        readonly regionCount : number,
        readonly cellSize : number,
        readonly cells : Array<VisualCell>
    ) {
    }

    cellAtLocation(point: Point) : VisualCell {
        return this.cells.find(vc => vc.contains(point));
    }
}