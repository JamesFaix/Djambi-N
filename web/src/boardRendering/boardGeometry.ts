import { Point, TransformMatrix } from "../geometry/model";
import { CellView, BoardView } from "./model";
import Geometry from "../geometry/geometry";

export default class BoardGeometry {

    //---CELL VIEW---

    public static cellTranslate(c : CellView, offset: Point) : CellView {
        return {
            id: c.id,
            type: c.type,
            state: c.state,
            polygons: c.polygons.map(p => Geometry.polygonTranslate(p, offset))
        };
    }

    public static cellTransform(c: CellView, matrix : TransformMatrix) : CellView {
        return {
            id: c.id,
            type: c.type,
            state: c.state,
            polygons: c.polygons.map(p => Geometry.polygonTransform(p, matrix))
        };
    }

    public static cellContains(c : CellView, point : Point) : boolean {
        return c.polygons.find(p => Geometry.polygonContains(p, point)) !== undefined;
    }

    public static cellToString(c : CellView) : string {
        return c.id.toString();
    }

    public static cellCentroid(c : CellView) : Point {
        let sumX = 0;
        let sumY = 0;
        let n = c.polygons.length;

        for (var i = 0; i < n; i++){
            let p = Geometry.polygonCentroid(c.polygons[i]);
            sumX += p.x;
            sumY += p.y;
        }

        return { x: sumX/n, y: sumY/n };
    }

    //---BOARD VIEW---

    public static boardCellAtPoint(b : BoardView, point : Point) : CellView {
        return b.cells.find((c: CellView) => this.cellContains(c, point));
    }

    public static boardCellById(b : BoardView, id : number) : CellView {
        return b.cells.find((c: CellView) => c.id === id);
    }
}