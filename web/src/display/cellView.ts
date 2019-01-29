import { CellState } from "./cellState";
import Color from "./color";
import { Polygon, Point, TransformMatrix } from "../geometry/model";
import GeometryService from "../geometry/geometryService";

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
            this.polygons.map(p => GeometryService.polygonTranslate(p, offset)),
            this.id,
            this.color);
    }

    transform(matrix : TransformMatrix) : CellView {
        return new CellView(
            this.polygons.map(p => GeometryService.polygonTransform(p, matrix)),
            this.id,
            this.color);
    }

    //#endregion

    contains(point : Point) : boolean {
        return this.polygons.find(p => GeometryService.polygonContains(p, point)) !== undefined;
    }

    toString() : string {
        return this.id.toString();
    }

    centroid() : Point {
        let sumX = 0;
        let sumY = 0;
        let n = this.polygons.length;

        for (var i = 0; i < n; i++){
            let c = GeometryService.polygonCentroid(this.polygons[i]);
            sumX += c.x;
            sumY += c.y;
        }

        return { x: sumX/n, y: sumY/n };
    }
}