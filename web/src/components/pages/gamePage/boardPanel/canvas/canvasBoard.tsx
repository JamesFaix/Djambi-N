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
    magnification : number
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    private readonly magnifiedBoard : BoardView;

    constructor(props: CanvasBoardProps) {
        super(props);

        const transform = Geometry.transformScale(props.magnification, props.magnification);
        const bv = BoardGeometry.boardTransform(props.board, transform);
        this.magnifiedBoard = bv;
    }

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
                    polygon={this.magnifiedBoard.polygon}
                    strokeColor={this.props.theme.getCellBaseColor(CellType.Center)}
                    strokeWidth={10}
                />
            </Layer>
        )
    }

    private renderCells() {
        return (
            <Layer>
                {
                    this.magnifiedBoard.cells.map((c, i) =>
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
                    this.magnifiedBoard.cells
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
        const canvasSize = BoardGeometry.boardDiameter(this.magnifiedBoard);

        return (
            <Stage width={canvasSize} height={canvasSize}>
                {this.renderBackground()}
                {this.renderCells()}
                {this.renderPieces()}
            </Stage>
        );
    }
}