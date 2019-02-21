import * as React from 'react';
import { Stage, Layer, Group, Text } from 'react-konva';
import { BoardView, CellView, CellType, PieceView } from '../../../../../boardRendering/model';
import ThemeService from '../../../../../themes/themeService';
import CanvasCell from './canvasCell';
import CanvasPiece from './canvasPiece';
import { Point } from '../../../../../boardRendering/model';
import Geometry from '../../../../../boardRendering/geometry';
import CanvasPolygon from './canvasPolygon';
import CanvasLabel from './canvasLabel';
import Debug from '../../../../../debug';

export interface CanvasBoardProps {
    board : BoardView,
    theme : ThemeService,
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
                    strokeColor={this.props.theme.getCellBaseColor(CellType.Center)}
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
                            theme={this.props.theme}
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
                                theme={this.props.theme}
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