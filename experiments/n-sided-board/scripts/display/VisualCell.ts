import {Point} from "../geometry/Point.js";
import {Polygon} from "../geometry/Polygon.js";
import {CellState} from "./CellState.js";
import {Color} from "./Color.js";
import {Cell} from "../rules/model/Cell.js";

export class VisualCell {
    state : CellState;

    constructor(
        readonly polygons : Array<Polygon>,
        readonly cell : Cell,
        readonly color : Color
    ) {
        this.state = CellState.Default;
    }

    translate(offset : Point) : VisualCell {
        return new VisualCell(
            this.polygons.map(p => p.translate(offset)), 
            this.cell, 
            this.color);
    }

    transform(matrix : Array<Array<number>>) : VisualCell {
        return new VisualCell(
            this.polygons.map(p => p.transform(matrix)),
            this.cell, 
            this.color);
    }

    contains(point : Point) : boolean {
        return this.polygons.find(p => p.contains(point)) !== undefined;
    }

    toString() : string {
        return this.cell.toString();
    }
}