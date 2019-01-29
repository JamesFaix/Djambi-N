import CellView from "./cellView";
import Point from "../geometry/point";

export default class BoardView {
    constructor (
        readonly regionCount : number,
        readonly cellSize : number,
        readonly cells : Array<CellView>
    ) {
    }

    cellAtPoint(point: Point) : CellView {
        return this.cells.find(vc => vc.contains(point));
    }

    cellById(id : number) : CellView {
        return this.cells.find(vc => vc.id === id);
    }
}