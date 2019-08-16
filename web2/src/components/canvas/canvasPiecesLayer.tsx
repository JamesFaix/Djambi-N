import * as React from 'react';
import { Layer } from 'react-konva';
import { CellView, BoardView, Point } from '../../viewModel/board/model';
import CanvasPiece from './canvasPiece';
import Geometry from '../../viewModel/board/geometry';

export interface CanvasPiecesLayerProps {
    board : BoardView,
    selectCell : (cell : CellView) => void,
    scale : number
}

export default class CanvasPiecesLayer extends React.Component<CanvasPiecesLayerProps> {
    render() {
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

    private getPieceSize() : number {
        return this.props.scale / this.props.board.cellCountPerSide / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.add(cellCenter, offset);
    }
}