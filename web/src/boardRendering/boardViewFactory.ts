import {
    BoardView,
    CellState,
    CellType,
    CellView,
    Line,
    Point,
    Polygon,
    } from "./model";
import Geometry from "./geometry";
import {
    Board,
    Location
    } from "../api/model";
import { List } from "../collections";
const Point = Geometry.Point;
const Line = Geometry.Line;
const Polygon = Geometry.Polygon;
const Location = Geometry.Location;

export default class BoardViewFactory {
    //--- Empty boardview creation ---

    //TODO: Add unit tests
    public static getRegionPolygon(boardPolygon : Polygon, regionNumber : number) : Polygon {
        const boardEdges = Polygon.edges(boardPolygon);
        const boardCentroid = Polygon.centroid(boardPolygon);
        const regionCount = boardEdges.length;

        return Polygon.create([
                boardPolygon.vertices[regionNumber],
                Line.midPoint(boardEdges[regionNumber]),
                boardCentroid,
                Line.midPoint(boardEdges[
                    (regionNumber + (regionCount-1)) % regionCount
                ])
            ]);
    }

    //TODO: Add unit tests
    public static getRowOrColumnBorderDistanceFromRegionEdge(
        rowOrCol : number,
        isLowerBorder : boolean,
        cellCountPerSide : number) : number {

        const borderNumber = isLowerBorder
            ? rowOrCol
            : rowOrCol + 1;

        const borderDistance = borderNumber === 0
            ? 0
            : (2 * borderNumber) - 1;

        /*
         * If there are n cells per side, then there are n/2 cells per side in each region.
         * The first row/column should be half cells, split with the neighboring region.
         * Further rows/columns will be whole cells.
         *
         * borderNumber | borderDistance
         * -------------|---------------
         *            0 | 0
         *            1 | 1 <-- only increase by 1 because 1/2 cells
         *            2 | 3 <-- start increasing by 2 each border
         *            3 | 5
         *            4 | 7
         *            5 | 9
         *
         * Assuming 9 cells per side
         *
         * borderNumber | result
         * -------------|-------
         *            0 | 1-(0/9) = 1
         *            1 | 1-(1/9) = 8/9
         *            2 | 1-(3/9) = 6/9 = 2/3
         *            3 | 1-(5/9) = 4/9
         *            4 | 1-(7/9) = 2/9
         *            5 | 1-(9/9) = 0
         */

        return 1 - (borderDistance / cellCountPerSide);
    }

    //TODO: Add unit tests
    public static getRowBorders(
        regionPolygon : Polygon,
        locationY : number,
        cellCountPerSide : number) : Line[] {

        const edges = Polygon.edges(regionPolygon);
        const lowerFraction = this.getRowOrColumnBorderDistanceFromRegionEdge(locationY, true, cellCountPerSide);
        const upperFraction = this.getRowOrColumnBorderDistanceFromRegionEdge(locationY, false, cellCountPerSide);

        return [
            Line.create(
                Line.fractionPoint(edges[3], 1-lowerFraction),
                Line.fractionPoint(edges[1], lowerFraction)
            ),
            Line.create(
                Line.fractionPoint(edges[3], 1-upperFraction),
                Line.fractionPoint(edges[1], upperFraction)
            )
        ];
    }

    //TODO: Add unit tests
    public static getCellPolygon(
        rowBorders : Line[],
        locationX : number,
        cellCountPerSide : number) : Polygon {

        const lowerFraction = this.getRowOrColumnBorderDistanceFromRegionEdge(locationX, true, cellCountPerSide);
        const upperFraction = this.getRowOrColumnBorderDistanceFromRegionEdge(locationX, false, cellCountPerSide);

        return Polygon.create([
                Line.fractionPoint(rowBorders[0], lowerFraction),
                Line.fractionPoint(rowBorders[0], upperFraction),
                Line.fractionPoint(rowBorders[1], upperFraction),
                Line.fractionPoint(rowBorders[1], lowerFraction),
            ]);
    }

    //TODO: Add unit tests
    public static getCellView(
        board : Board,
        rowBorders : Line[],
        location : Location,
        cellCountPerSide : number) : CellView {

        const polygon = this.getCellPolygon(rowBorders, location.x, cellCountPerSide);

        const cell = board.cells
            .find(c => List.exists(c.locations, loc => Location.equals(loc, location)));

        return {
            id: cell.id,
            locations: cell.locations,
            type: this.getCellType(location),
            state: CellState.Default,
            piece: null,
            polygon: polygon
        }
    }

    //TODO: Add unit tests
    public static createEmptyBoardView(board : Board): BoardView {
        const cellCountPerSide = (board.regionSize * 2) - 1;
        const boardPolygon = Geometry.RegularPolygon.create(board.regionCount, 1);
        let cellViews : Array<CellView> = [];

        for (var region = 0; region < board.regionCount; region++) {
            const regionPolygon = this.getRegionPolygon(boardPolygon, region);

            for (var y = 0; y < board.regionSize; y++) {
                const rowBorders = this.getRowBorders(regionPolygon, y, cellCountPerSide);

                for (var x = 0; x < board.regionSize; x++) {
                    const location = Location.create(region,x,y);
                    const cv = this.getCellView(board, rowBorders, location, cellCountPerSide);
                    cellViews.push(cv);
                }
            }
        }

        cellViews = this.mergePartialCellViews(cellViews);

        return {
            regionCount: board.regionCount,
            cellCountPerSide: cellCountPerSide,
            cells: cellViews,
            polygon: boardPolygon,
        };
    }

    //TODO: Add tests
    public static mergePartialCellViews(cells : CellView[]) : CellView[] {
        return List.mergeMatches(
            cells,
            (a, b) => a.id === b.id,
            (a, b) => this.mergeCellViews(a, b));
    }

    //TODO: Add tests
    public static mergeCellViews(a : CellView, b: CellView) : CellView {
        if (a.piece !== b.piece) {
            throw "Cannot merge CellViews with difference pieces";
        }

        return {
            id: a.id,
            locations: a.locations.concat(b.locations),
            type: a.type,
            state: a.state,
            piece: a.piece,
            polygon: this.mergePolygons([a.polygon, b.polygon])
        };
    }

    //TODO: Add tests; fix bugs
    public static mergePolygons(polygons : Polygon[]) : Polygon {
        if (polygons.length === 0) {
            throw "Cannot merge 0 polygons.";
        }

        if (polygons.length === 1) {
            return polygons[0];
        }

        const threshold = 0.00001;

        //Get all edges of each polygon
        const edges = List.flatMap(polygons, Polygon.edges);

        //Group by which are the same line segment
        const groupedEdges = List.groupMatches(
            edges,
            (a, b) => Line.isCloseTo(a, b, threshold));

        //Filter out any edges that are shared by 2 polygons
        let resultEdges = groupedEdges
            .filter(g => g.length === 1)
            .map(g => g[0]);

        const vertices : Point[] = [];

        //Add the vertices of the first edge
        let e = resultEdges.pop();
        vertices.push(e.a);
        vertices.push(e.b);

        //Loop until there is one edge left
        //The last edge does not contain any new vertices,
        //it just links the first and last ones already in the array
        while (resultEdges.length > 1) {
            //Find an edge that is chained to the previous vertex, and remove it from the list
            e = resultEdges.find(e2 => Line.isChainedTo(e, e2, threshold));
            resultEdges = resultEdges.filter(e2 => !Line.isCloseTo(e, e2, threshold));

            //Add the vertex not already in the list
            let nextVertex = List.exists(vertices, p => Point.isCloseTo(p, e.a, threshold)) ? e.b : e.a;
            vertices.push(nextVertex);
        }

        return Polygon.create(vertices);
    }

    public static getCellType(location : Location) : CellType {
        if (location.x === 0 && location.y === 0){
            return CellType.Center;
        }
        if ((location.x + location.y) % 2 === 1){
            return CellType.Odd;
        }
        return CellType.Even;
    }
}