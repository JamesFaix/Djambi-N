import * as React from 'react';
import { Layer } from 'react-konva';
import { CellView, BoardView, Point } from '../../viewModel/board/model';
import CanvasPiece from './canvasPiece';
import Geometry from '../../viewModel/board/geometry';
import { PieceKind } from '../../api/model';

export interface CanvasPiecesLayerStyle {
    scale : number
}

export interface CanvasPiecesLayerProps {
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasPiecesLayerStyle,
    images : Map<PieceKind, HTMLImageElement>
}

export default class CanvasPiecesLayer extends React.Component<CanvasPiecesLayerProps> {
    render() {
        const size = this.getPieceSize();

        return (
            <Layer>
                {
                    this.props.board.cells
                        .filter(c => c.piece !== null)
                        .map((c, i) => {
                            const image = this.props.images.get(c.piece.kind);
                            if (image) {
                                return (
                                    <CanvasPiece
                                        key={"piece" + i}
                                        piece={c.piece}
                                        onClick={() => this.props.selectCell(c)}
                                        size={size}
                                        location={this.getPieceLocation(c)}
                                        image={image}
                                    />
                                );
                            } else {
                                return null;
                            }
                        })
                }
            </Layer>
        );
    }

    private getPieceSize() : number {
        return this.props.style.scale / this.props.board.cellCountPerSide / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.add(cellCenter, offset);
    }
}