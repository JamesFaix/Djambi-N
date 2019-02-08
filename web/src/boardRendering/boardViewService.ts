import { Location, Board, Game } from "../api/model";
import { Polygon, Line } from "../geometry/model";
import Geometry from "../geometry/geometry";
import { BoardView, CellView, CellType, CellState } from "./model";
import BoardGeometry from "./boardGeometry";
import * as Sprintf from 'sprintf-js';

export default class BoardViewService {
    private readonly emptyBoardViewCache : any = {};

    public getBoard(board : Board, cellSize : number, game : Game) : BoardView {
        const key = Sprintf.sprintf("%i-%i-%i", board.regionCount, board.regionSize, cellSize);
        let empty = this.emptyBoardViewCache[key];
        if (!empty) {
            empty = BoardViewService.createEmptyBoard(board, cellSize);
        }
        return BoardViewService.update(empty, game);
    }

    //--- Empty board creation ---

    private static createEmptyBoard(board : Board, cellSize : number): BoardView {
        const cellCountPerSide = (board.regionSize * 2) - 1;
        const sideLength = cellCountPerSide * cellSize;

        let cellViews : Array<CellView> = [];

        const boardPolygon = this.getBoardPolygon(board.regionCount, sideLength);
        const boardEdges = Geometry.polygonEdges(boardPolygon);
        const boardCentroid = Geometry.polygonCentroid(boardPolygon);

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

                    const cv : CellView = {
                        id: cellId,
                        type: this.getCellType(col, row),
                        state: CellState.Default,
                        piece: null,
                        polygons: [polygon]
                    }

                    cellViews.push(cv);
                }
            }
        }

        cellViews = this.coalesceColocatedCells(cellViews);

        let boardView = {
            regionCount: board.regionCount,
            cellSize: cellSize,
            cells: cellViews,
            polygon: boardPolygon
        };

        const transform = Geometry.transformRotation(360 / board.regionCount / 2);
        const radius = this.getPolygonRadius(board.regionCount, sideLength);
        const offset = { x: radius, y: radius };

        boardView = BoardGeometry.boardTransform(boardView, transform);
        boardView = BoardGeometry.boardTranslate(boardView, offset);

        return boardView;
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
                piece: group[0].piece,
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

    //--- Update board ---

    private static update(board : BoardView, game : Game) : BoardView {

        const newCells = board.cells.map(c => {
            let newState = CellState.Default;
            const turn = game.currentTurn;

            if (turn) {
                if (turn.selections.find(s => s.cellId === c.id)) {
                    newState = CellState.Selected;
                } else if (turn.selectionOptions.find(cellId => cellId === c.id)) {
                    newState = CellState.Selectable;
                }
            }

            const piece = game.pieces.find(p => p.cellId === c.id);
            const owner = piece ? game.players.find(p => p.id === piece.playerId) : null;
            const colorId = owner ? owner.colorId : null;
            const pieceView = piece ? { kind: piece.kind, colorId: colorId } : null;

            return {
                id: c.id,
                type: c.type,
                state: newState,
                piece: pieceView,
                polygons: c.polygons
            }
        });

        return {
            regionCount : board.regionCount,
            cellSize : board.cellSize,
            cells : newCells,
            polygon : board.polygon
        }
    }
}