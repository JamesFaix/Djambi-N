import * as React from 'react';
import CanvasBoard, { CanvasBoardStyle } from '../canvas/canvasBoard';
import { BoardView, CellView } from '../../viewModel/board/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

export interface BoardSectionProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void
}

class boardSection extends React.Component<BoardSectionProps> {
    render(){
        const boardStyle : CanvasBoardStyle = {
            width: 1000, //get from CanvasTransformService
            height : 1000, //get from CanvasTransformService
            strokeWidth : 5, //5 maybe put in settings somewhere
            strokeColor : "black", //pull from Colors module
            scale : 500 //get from CanvasTransformService
        };

        if (!this.props.board) {
            return null;
        }

        return (
            <div>
                <CanvasBoard
                    style={boardStyle}
                    gameId={this.props.gameId}
                    board={this.props.board}
                    selectCell={this.props.selectCell}
                />
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    const game = state.activeGame.game;
    return {
        gameId: game ? game.id : null,
        board: state.activeGame.boardView
    };
}

const mapDispatchToProps = (dispatch: Dispatch, ownProps : any) => {
    return {
        selectCell: (cell : CellView) => ThunkActions.selectCell(ownProps.activeGame.game.id, cell.id)(dispatch)
    };
}

const BoardSection = connect(mapStateToProps, mapDispatchToProps)(boardSection);
export default BoardSection;