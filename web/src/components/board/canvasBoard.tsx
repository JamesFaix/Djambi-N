import * as React from 'react';
import { Stage, Layer } from 'react-konva';
import { BoardView, CellView } from '../../boardRendering/model';
import ThemeService from '../../themes/themeService';
import CanvasCell from './canvasCell';
import CanvasPiece from './canvasPiece';
import { Point } from '../../geometry/model';
import BoardGeometry from '../../boardRendering/boardGeometry';
import Geometry from '../../geometry/geometry';

export interface CanvasBoardProps {
    board : BoardView,
    theme : ThemeService,
    selectCell : (cell : CellView) => void
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {

    private getPieceSize() : number {
        return this.props.board.cellSize / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = BoardGeometry.cellCentroid(cell);
        const offset = { x: size, y: size };
        return Geometry.pointTranslate(cellCenter, offset);
    }

    render() {
        return (
            <Stage width={1000} height={1000}>
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
                <Layer>
                    {
                        this.props.board.cells
                            .filter(c => c.piece !== null)
                            .map(c =>
                                <CanvasPiece
                                    piece={c.piece}
                                    theme={this.props.theme}
                                    onClick={() => this.props.selectCell(c)}
                                    size={this.props.board.cellSize/2}
                                    location={this.getPieceLocation(c)}
                                />
                            )
                    }
                </Layer>
            </Stage>
        );
    }
}