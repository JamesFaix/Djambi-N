import { Point } from "../geometry/model";
import { CellView, BoardView } from "./model";
import Geometry from "../geometry/geometry";
import * as MathJs from 'mathjs';

export default class BoardGeometry {

    //---CELL VIEW---

    public static cellTranslate(c : CellView, offset: Point) : CellView {
        return {
            id: c.id,
            type: c.type,
            state: c.state,
            piece: c.piece,
            polygons: c.polygons.map(p => Geometry.polygonTranslate(p, offset))
        };
    }

    public static cellTransform(c: CellView, matrix : MathJs.Matrix) : CellView {
        return {
            id: c.id,
            type: c.type,
            state: c.state,
            piece: c.piece,
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

    public static boardTransform(b : BoardView, matrix : MathJs.Matrix) : BoardView {
        return {
            regionCount: b.regionCount,
            cellCountPerSide: b.cellCountPerSide,
            polygon: Geometry.polygonTransform(b.polygon, matrix),
            cells: b.cells.map(c => this.cellTransform(c, matrix))
        };
    }

    public static boardTranslate(b : BoardView, offset : Point) : BoardView {
        return {
            regionCount: b.regionCount,
            cellCountPerSide: b.cellCountPerSide,
            polygon: Geometry.polygonTranslate(b.polygon, offset),
            cells: b.cells.map(c => this.cellTranslate(c, offset))
        };
    }

    public static boardSize(b : BoardView) : Point {
        const h = Geometry.polygonHeight(b.polygon);
        const w = Geometry.polygonWidth(b.polygon);
        return { x: w, y: h };
    }
}