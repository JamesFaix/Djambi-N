import { CellState } from "./cellState";
import Color from "./color";
import { Polygon, Point, TransformMatrix } from "../geometry/model";
import Geometry from "../geometry/geometry";

export default class CellView {
    state : CellState;

    constructor(
        readonly polygons : Polygon[],
        readonly id : number,
        readonly color : Color
    ) {
        this.state = CellState.Default;
    }

    //#region Transformable

    translate(offset : Point) : CellView {
        return new CellView(
            this.polygons.map(p => Geometry.polygonTranslate(p, offset)),
            this.id,
            this.color);
    }

    transform(matrix : TransformMatrix) : CellView {
        return new CellView(
            this.polygons.map(p => Geometry.polygonTransform(p, matrix)),
            this.id,
            this.color);
    }

    //#endregion

    contains(point : Point) : boolean {
        return this.polygons.find(p => Geometry.polygonContains(p, point)) !== undefined;
    }

    toString() : string {
        return this.id.toString();
    }

    centroid() : Point {
        let sumX = 0;
        let sumY = 0;
        let n = this.polygons.length;

        for (var i = 0; i < n; i++){
            let c = Geometry.polygonCentroid(this.polygons[i]);
            sumX += c.x;
            sumY += c.y;
        }

        return { x: sumX/n, y: sumY/n };
    }
}