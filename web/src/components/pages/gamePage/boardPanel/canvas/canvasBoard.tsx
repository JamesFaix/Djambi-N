import * as React from 'react';
import { Stage, Layer } from 'react-konva';
import { BoardView, CellView, CellType } from '../../../../../boardRendering/model';
import CanvasCell from './canvasCell';
import CanvasPiece from './canvasPiece';
import { Point } from '../../../../../boardRendering/model';
import Geometry from '../../../../../boardRendering/geometry';
import CanvasPolygon from './canvasPolygon';
import CanvasLabel from './canvasLabel';
import Debug from '../../../../../debug';
import { Kernel as K } from '../../../../../kernel';

export interface CanvasBoardProps {
    board : BoardView,
    selectCell : (cell : CellView) => void,
    scale : number,
    boardStrokeWidth : number,
    size : Point
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    private getPieceSize() : number {
        return this.props.scale / this.props.board.cellCountPerSide / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.translate(cellCenter, offset);
    }

    private renderBackground() {
        return (
            <Layer>
                <CanvasPolygon
                    polygon={this.props.board.polygon}
                    strokeColor={K.theme.getCellBaseColor(CellType.Center)}
                    strokeWidth={this.props.boardStrokeWidth}
                />
            </Layer>
        )
    }

    private renderCells() {
        return (
            <Layer>
                {
                    this.props.board.cells.map((c, i) =>
                        <CanvasCell
                            key={"cell" + i}
                            cell={c}
                            selectCell={(cell) => this.props.selectCell(cell)}
                        />
                    )
                }
            </Layer>
        );
    }

    private renderPieces() {
        const size = this.getPieceSize();
        return (
            <Layer>
                {
                    this.props.board.cells
                        .filter(c => c.piece !== null)
                        .map((c, i) =>
                            <CanvasPiece
                                key={"piece" + i}
                                piece={c.piece}
                                onClick={() => this.props.selectCell(c)}
                                size={size}
                                location={this.getPieceLocation(c)}
                            />
                        )
                }
            </Layer>
        );
    }

    private renderDebugLabels() {
        if (!Debug.showPieceAndCellIds){
            return undefined;
        }

        return (
            <Layer>
                {
                    this.props.board.cells.map((c, i) =>
                        <CanvasLabel
                            key={"label" + i}
                            cell={c}
                            onClick={() => this.props.selectCell(c)}
                        />
                    )
                }
            </Layer>
        );
    }

    render() {
        return (
            <Stage width={this.props.size.x} height={this.props.size.y}>
                {this.renderBackground()}
                {this.renderCells()}
                {this.renderPieces()}
                {this.renderDebugLabels()}
            </Stage>
        );
    }
}