/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import {Point} from "../geometry/Point.js";
import {Line} from "../geometry/Line.js";
import {Polygon} from "../geometry/Polygon.js";
import {Transforms} from "../geometry/Transforms.js";
import {VisualCell} from "./VisualCell.js";
import {Location} from "../apiClient/model/Location.js";
import {BoardClient} from "../apiClient/BoardClient.js";
import {Color} from "./Color.js";
import {VisualBoard} from "./VisualBoard.js";

export class VisualBoardFactory {
    constructor(){

    }

    static async createBoard(regionCount : number, cellSize : number): Promise<VisualBoard> {
        let board = await BoardClient.getBoard(regionCount);

        const cellCountPerSide = (board.regionSize * 2) - 1;
        const sideLength = cellCountPerSide * cellSize;

        let visualCells : Array<VisualCell> = [];
        
        let boardPolygon = VisualBoardFactory.getBoardPolygon(regionCount, sideLength);
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
    
            for (var row = 0; row < board.regionSize; row++) {
                let lowerFraction = getFraction(row, true);
                let upperFraction = getFraction(row, false);
    
                const rowBorders = [
                    new Line(regionEdges[3].fractionPoint(1-lowerFraction), regionEdges[1].fractionPoint(lowerFraction)),
                    new Line(regionEdges[3].fractionPoint(1-upperFraction), regionEdges[1].fractionPoint(upperFraction)),                
                ];
    
                for (var col = 0; col < board.regionSize; col++) {        
                    lowerFraction = getFraction(col, true);
                    upperFraction = getFraction(col, false);
        
                    const location = new Location(col, row, i);
                    
                    const polygon = new Polygon(
                        [
                            rowBorders[0].fractionPoint(lowerFraction),
                            rowBorders[0].fractionPoint(upperFraction),
                            rowBorders[1].fractionPoint(upperFraction),
                            rowBorders[1].fractionPoint(lowerFraction),
                        ]);

                    const cellId = board.cells
                        .find(c => c.locations
                            .find(loc => VisualBoardFactory.locationEquals(loc, location)) 
                            !== undefined)
                        .id;

                    const visualCell = new VisualCell(
                        [polygon], 
                        cellId, 
                        VisualBoardFactory.getCellColor(col, row));
    
                    visualCells.push(visualCell);
                }
            }
        }
        
        visualCells = VisualBoardFactory.coalesceColocatedCells(visualCells);

        const transform = Transforms.rotation(360 / regionCount / 2);
        visualCells = visualCells.map(c => c.transform(transform));

        const radius = VisualBoardFactory.getPolygonRadius(regionCount, sideLength);
        const offset = new Point(radius, radius);
        visualCells = visualCells.map(c => c.translate(offset));

        return new VisualBoard(regionCount, cellSize, visualCells);
    }

    private static locationEquals(a : Location, b : Location) : boolean {
        return a.x === b.x 
            && a.y === b.y 
            && a.region === b.region;
    }
    
    private static coalesceColocatedCells(cells : Array<VisualCell>) : Array<VisualCell> {
        const results : Array<VisualCell> = [];

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
            const coalesced = new VisualCell(
                group.map((vc : VisualCell) => vc.polygons)
                    .reduce((a : Polygon[], b : Polygon[]) => a.concat(b)),
                group[0].id,
                group[0].color
            );
            results.push(coalesced);
        });

        return results;
    }
        
    private static getBoardPolygon(sideCount : number, sideLength : number) : Polygon {
        const centralAngle = Math.PI * 2 / sideCount;
        const radius = VisualBoardFactory.getPolygonRadius(sideCount, sideLength);
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

    private static getPolygonRadius(sideCount : number, sideLength : number) : number {
        const centralAngle = Math.PI * 2 / sideCount;
        const outerAngle = (Math.PI - centralAngle) / 2;
        return sideLength * Math.sin(outerAngle) / Math.sin(centralAngle);    
    }

    private static getCellColor(col : number, row : number) : Color {
        if (col === 0 && row === 0){
            return Color.gray();
        }
        if ((col + row) % 2 === 1){
            return Color.black();
        }
        return Color.white();
    }
}