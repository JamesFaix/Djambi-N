import {Point} from "../geometry/Point.js";
import {Line} from "../geometry/Line.js";
import {Polygon} from "../geometry/Polygon.js";
import {Transforms} from "../geometry/Transforms.js";
import {VisualCell} from "./VisualCell.js";
import {Location} from "../rules/model/Location.js";
import {Landscape} from "../rules/model/Landscape.js";
import {Color} from "./Color.js";
import {Cell} from "../rules/model/Cell.js";

export class VisualBoard {
    readonly cells : Array<VisualCell>;
    readonly landscape : Landscape;
    constructor (
        readonly regionCount : number,
        readonly cellSize : number,
        readonly cellCountPerSide : number
    ) {
        const sideLength = cellCountPerSide * cellSize;
        const regionCellCount = Math.ceil(cellCountPerSide / 2);
    
        this.landscape = new Landscape(regionCount, regionCellCount);

        let cells : Array<VisualCell> = [];
        
        let boardPolygon = this.getBoardPolygon(regionCount, sideLength);
        let boardEdges = boardPolygon.edges();
        let boardCentroid = boardPolygon.centroid();
    
        //Loop over regions
        for (var i = 0; i < regionCount; i++) {
    
            const regionVertices = [
                boardPolygon.vertices[i],
                boardEdges[i].midpoint(),
                boardCentroid,
                boardEdges[(i+(regionCount - 1))%regionCount].midpoint()
            ];
    
            const region = new Polygon(regionVertices);
            const regionEdges = region.edges();
    
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
    
            for (var row = 0; row < regionCellCount; row++) {
                let lowerFraction = getFraction(row, true);
                let upperFraction = getFraction(row, false);
    
                const rowBorders = [
                    new Line(regionEdges[3].fractionPoint(1-lowerFraction), regionEdges[1].fractionPoint(lowerFraction)),
                    new Line(regionEdges[3].fractionPoint(1-upperFraction), regionEdges[1].fractionPoint(upperFraction)),                
                ];
    
                for (var col = 0; col < regionCellCount; col++) {        
                    lowerFraction = getFraction(col, true);
                    upperFraction = getFraction(col, false);
        
                    const polygon = new Polygon(
                        [
                            rowBorders[0].fractionPoint(lowerFraction),
                            rowBorders[0].fractionPoint(upperFraction),
                            rowBorders[1].fractionPoint(upperFraction),
                            rowBorders[1].fractionPoint(lowerFraction),
                        ]);

                    const cell = new VisualCell(
                        [polygon], 
                        new Cell([new Location(col, row, i)]), 
                        this.getCellColor(col, row));
    
                    cells.push(cell);
                }
            }
        }
        
        cells = this.coalesceColocatedCells(cells);

        const transform = Transforms.rotation(360 / regionCount / 2);
        cells = cells.map(c => c.transform(transform));

        const radius = this.getPolygonRadius(regionCount, sideLength);
        const offset = new Point(radius, radius);
        cells = cells.map(c => c.translate(offset));

        this.cells = cells;
    }

    private coalesceColocatedCells(cells : Array<VisualCell>) : Array<VisualCell> {
        let results : Array<VisualCell> = [];

        while (cells.length > 0) {
            let vc = cells[0];

            //Get all colocations for all locations of the VisualCell
            let coLocations = vc.cell.locations
                .map(loc => this.landscape.colocations(loc))
                .reduce((a, b) => a.concat(b));

            //Get all VisualCells that have a location in the colocation list
            let coCells = cells.filter(vc1 => 
                coLocations.find(l => vc1.cell.locations[0].equals(l)) 
                !== undefined);

            vc = new VisualCell(
                coCells.map(cc => cc.polygons).reduce((a, b) => a.concat(b)), 
                new Cell(coLocations), 
                vc.color);

            results.push(vc);

            //Remove all colocated cells from list
            cells = cells.filter(vc1 => 
                coLocations.find(l => vc1.cell.locations[0].equals(l))
                === undefined);
        }

        return results;
    }
        
    private getBoardPolygon(sideCount : number, sideLength : number) : Polygon {
        const centralAngle = Math.PI * 2 / sideCount;
        const radius = this.getPolygonRadius(sideCount, sideLength);
        const centroid = new Point(0, 0);

        const vertices = [];

        for (var i = 0; i < sideCount; i++) {
            let v = new Point(
                centroid.x + (radius * Math.sin(centralAngle * i)), 
                centroid.y + (radius * Math.cos(centralAngle * i))
            );
            vertices.push(v);
        }

        return new Polygon(vertices);
    }

    private getPolygonRadius(sideCount : number, sideLength : number) : number {
        const centralAngle = Math.PI * 2 / sideCount;
        const outerAngle = (Math.PI - centralAngle) / 2;
        return sideLength * Math.sin(outerAngle) / Math.sin(centralAngle);    
    }

    private getCellColor(col : number, row : number) : Color {
        if (col === 0 && row === 0){
            return Color.gray();
        }
        if ((col + row) % 2 === 1){
            return Color.black();
        }
        return Color.white();
    }

    cellAtLocation(point: Point) : VisualCell {
        return this.cells.find(vc => vc.contains(point));
    }

    cellNeighbors(cell : VisualCell) : Array<VisualCell> {
        return this.landscape.neighbors(cell.cell)
            .map(c => this.cells.find(vc => vc.cell.is(c)));
    }

    cellPaths(cell : VisualCell) : Array<Array<VisualCell>> {
        return this.landscape.getPaths(cell.cell)
            .map(path => path
                .map(c => this.cells.find(vc => vc.cell.is(c))));
    }
}