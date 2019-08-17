import * as React from 'react';
import { Layer } from 'react-konva';
import { CellView, BoardView } from '../../viewModel/board/model';
import Debug from '../../debug';
import CanvasLabel from './canvasLabel';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

export interface CanvasLabelsLayerProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void
}

class canvasLabelsLayer extends React.Component<CanvasLabelsLayerProps> {
    render() {
        if (!Debug.showCellLabels){
            return null;
        }

        const board = this.props.board;

        return (
            <Layer>
                {
                    board.cells.map((c, i) =>
                        <CanvasLabel
                            key={"label" + i}
                            board={board}
                            cell={c}
                            onClick={() => this.props.selectCell(c)}
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

const CanvasLabelsLayer = connect(mapStateToProps, mapDispatchToProps)(canvasLabelsLayer);
export default CanvasLabelsLayer;