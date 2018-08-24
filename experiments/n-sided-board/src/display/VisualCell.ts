import {Point} from "../geometry/Point";
import {Polygon} from "../geometry/Polygon";
import {CellState} from "./CellState";
import {Color} from "./Color";

export class VisualCell {
    state : CellState;

    constructor(
        readonly polygons : Array<Polygon>,
        readonly id : number,
        readonly color : Color
    ) {
        this.state = CellState.Default;
    }

    translate(offset : Point) : VisualCell {
        return new VisualCell(
            this.polygons.map(p => p.translate(offset)), 
            this.id, 
            this.color);
    }

    transform(matrix : Array<Array<number>>) : VisualCell {
        return new VisualCell(
            this.polygons.map(p => p.transform(matrix)),
            this.id, 
            this.color);
    }

    contains(point : Point) : boolean {
        return this.polygons.find(p => p.contains(point)) !== undefined;
    }

    toString() : string {
        return this.id.toString();
    }

    centroid() : Point {
        let sumX = 0;
        let sumY = 0;
        let n = this.polygons.length;

        for (var i = 0; i < n; i++){
            let c = this.polygons[i].centroid();
            sumX += c.x;
            sumY += c.y;
        }

        return new Point(sumX/n, sumY/n);
    }
}