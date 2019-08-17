import * as React from 'react';
import { Layer } from 'react-konva';
import { CellView, BoardView, Point } from '../../viewModel/board/model';
import CanvasPiece from './canvasPiece';
import Geometry from '../../viewModel/board/geometry';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

export interface CanvasPiecesLayerStyle {
    scale : number
}

export interface CanvasPiecesLayerProps {
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasPiecesLayerStyle
}

class canvasPiecesLayer extends React.Component<CanvasPiecesLayerProps> {
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
        return this.props.style.scale / this.props.board.cellCountPerSide / 2;
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.getPieceSize();
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.add(cellCenter, offset);
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        gameId: state.activeGame.game.id,
        board: state.activeGame.boardView
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    const gameId : any = null;
    return {
        selectCell : (cell : CellView) => ThunkActions.selectCell(gameId, cell.id)(dispatch)
    };
}

const CanvasPiecesLayer = connect(mapStateToProps, mapDispatchToProps)(canvasPiecesLayer);
export default CanvasPiecesLayer;