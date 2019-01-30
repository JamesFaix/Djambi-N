import { Location, Board } from "../api/model";
import { Polygon, Line } from "../geometry/model";
import Geometry from "../geometry/geometry";
import { BoardView, CellView, CellType, CellState } from "./model";
import BoardGeometry from "./boardGeometry";

export default class BoardViewFactory {
    constructor(){

    }

    static createBoard(board : Board, cellSize : number): BoardView {

        const cellCountPerSide = (board.regionSize * 2) - 1;
        const sideLength = cellCountPerSide * cellSize;

        let cellViews : Array<CellView> = [];

        let boardPolygon = this.getBoardPolygon(board.regionCount, sideLength);
        let boardEdges = Geometry.polygonEdges(boardPolygon);
        let boardCentroid = Geometry.polygonCentroid(boardPolygon);

        //Loop over regions
        for (var i = 0; i < board.regionCount; i++) {

            const regionVertices = [
                boardPolygon.vertices[i],
                Geometry.lineMidPoint(boardEdges[i]),
                boardCentroid,
                Geometry.lineMidPoint(boardEdges[(i+(board.regionCount - 1))%board.regionCount])
            ];

            const region : Polygon = { vertices: regionVertices };
            const regionEdges = Geometry.polygonEdges(region);

            function getFraction(index : number, isLower : boolean) {
                let result = 0;

                if (!isLower){
                    index += 1;
                }

                if (index === 0) {
                    result = index / cellCountPerSide;
                } else{
                    result = (2*index -1) / cellCountPerSide;
                }

                return 1 - result;
            }

            for (var row = 0; row < board.regionSize; row++) {
                let lowerFraction = getFraction(row, true);
                let upperFraction = getFraction(row, false);

                const rowBorders : Line[] = [
                    {
                        a: Geometry.lineFractionPoint(regionEdges[3], 1-lowerFraction),
                        b: Geometry.lineFractionPoint(regionEdges[1], lowerFraction)
                    },
                    {
                        a: Geometry.lineFractionPoint(regionEdges[3], 1-upperFraction),
                        b: Geometry.lineFractionPoint(regionEdges[1], upperFraction)
                    }
                ];

                for (var col = 0; col < board.regionSize; col++) {
                    lowerFraction = getFraction(col, true);
                    upperFraction = getFraction(col, false);

                    const location : Location = {
                        x: col,
                        y: row,
                        region: i
                    };

                    const polygon : Polygon = {
                        vertices: [
                            Geometry.lineFractionPoint(rowBorders[0], lowerFraction),
                            Geometry.lineFractionPoint(rowBorders[0], upperFraction),
                            Geometry.lineFractionPoint(rowBorders[1], upperFraction),
                            Geometry.lineFractionPoint(rowBorders[1], lowerFraction),
                        ]
                    };

                    const cellId = board.cells
                        .find(c => c.locations
                            .find(loc => this.locationEquals(loc, location))
                            !== undefined)
                        .id;

                    const cv = {
                        id: cellId,
                        type: this.getCellType(col, row),
                        state: CellState.Default,
                        polygons: [polygon]
                    }

                    cellViews.push(cv);
                }
            }
        }

        cellViews = this.coalesceColocatedCells(cellViews);

        const transform = Geometry.transformRotation(360 / board.regionCount / 2);
        cellViews = cellViews.map(c => BoardGeometry.cellTransform(c, transform));

        const radius = this.getPolygonRadius(board.regionCount, sideLength);
        const offset = { x: radius, y: radius };
        cellViews = cellViews.map(c =>  BoardGeometry.cellTranslate(c, offset));

        return {
            regionCount: board.regionCount,
            cellSize: cellSize,
            cells: cellViews
        };
    }

    private static locationEquals(a : Location, b : Location) : boolean {
        return a.x === b.x
            && a.y === b.y
            && a.region === b.region;
    }

    private static coalesceColocatedCells(cells : Array<CellView>) : Array<CellView> {
        const results : Array<CellView> = [];

        const map = new Map();
        cells.forEach(vc => {
            const matches = map.get(vc.id);
            if (matches) {
                matches.push(vc);
            } else {
                map.set(vc.id, [vc]);
            }
        });

        map.forEach(group => {
            const coalesced : CellView = {
                id: group[0].id,
                type: group[0].type,
                state: group[0].state,
                polygons: group
                    .map((vc : CellView) => vc.polygons)
                    .reduce((a : Polygon[], b : Polygon[]) => a.concat(b)),
            };
            results.push(coalesced);
        });

        return results;
    }

    private static getBoardPolygon(sideCount : number, sideLength : number) : Polygon {
        const centralAngle = Math.PI * 2 / sideCount;
        const radius = this.getPolygonRadius(sideCount, sideLength);
        const centroid = { x: 0, y:0 };

        const vertices = [];

        for (var i = 0; i < sideCount; i++) {
            let v = {
                x: centroid.x + (radius * Math.sin(centralAngle * i)),
                y: centroid.y + (radius * Math.cos(centralAngle * i))
            };
            vertices.push(v);
        }

        return { vertices: vertices };
    }

    private static getPolygonRadius(sideCount : number, sideLength : number) : number {
        const centralAngle = Math.PI * 2 / sideCount;
        const outerAngle = (Math.PI - centralAngle) / 2;
        return sideLength * Math.sin(outerAngle) / Math.sin(centralAngle);
    }

    private static getCellType(col : number, row : number) : CellType {
        if (col === 0 && row === 0){
            return CellType.Center;
        }
        if ((col + row) % 2 === 1){
            return CellType.Odd;
        }
        return CellType.Even;
    }
}