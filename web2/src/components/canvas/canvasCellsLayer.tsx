import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCell from './canvasCell';
import { CellView, BoardView } from '../../viewModel/board/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

export interface CanvasCellsLayerProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void
}

class canvasCellsLayer extends React.Component<CanvasCellsLayerProps> {
    render() {
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

const CanvasCellLayer = connect(mapStateToProps, mapDispatchToProps)(canvasCellsLayer);
export default CanvasCellLayer;