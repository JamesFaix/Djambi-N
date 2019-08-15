import * as React from 'react';
import { Board } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';

interface LoadBoardProps {
    regionCount : number,
    boards : Map<number, Board>
    loadBoard : (regionCount : number) => void
}

class loadBoard extends React.Component<LoadBoardProps> {
    componentDidMount() {
        const board = this.props.boards.get(this.props.regionCount);
        if (!board) {
            this.props.loadBoard(this.props.regionCount);
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state : AppState) => {
    const game = state.activeGame.game;
    const params = game ? game.parameters : null;
    const rc = params ? params.regionCount : null;

    return {
        regionCount : rc,
        boards : state.boards.boards
    };
}

const mapDispatchToProps = () => {
    return function (dispatch : Dispatch) {
        return {
            loadBoard: (regionCount : number) => ThunkActions.loadBoard(regionCount)(dispatch)
        }
    };
}

const LoadBoard = connect(mapStateToProps, mapDispatchToProps)(loadBoard);

export default LoadBoard;