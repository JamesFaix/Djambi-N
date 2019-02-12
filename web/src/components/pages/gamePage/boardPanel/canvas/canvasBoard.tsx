import * as React from 'react';
import { Stage, Layer } from 'react-konva';
import { BoardView, CellView, CellType } from '../../../../../boardRendering/model';
import ThemeService from '../../../../../themes/themeService';
import CanvasCell from './canvasCell';
import CanvasPiece from './canvasPiece';
import { Point } from '../../../../../geometry/model';
import BoardGeometry from '../../../../../boardRendering/boardGeometry';
import Geometry from '../../../../../geometry/geometry';
import CanvasPolygon from './canvasPolygon';

export interface CanvasBoardProps {
    board : BoardView,
    theme : ThemeService,
    selectCell : (cell : CellView) => void,
    magnification : number,
    boardStrokeWidth : number,
    size : Point
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    private getPieceSize() : number {
        return this.props.magnification / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = BoardGeometry.cellCentroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.pointTranslate(cellCenter, offset);
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
                                size={this.getPieceSize()}
                                location={this.getPieceLocation(c)}
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
            </Stage>
        );
    }
}