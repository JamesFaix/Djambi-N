import * as React from 'react';
import { Game, Event } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';

interface LoadGameAndHistoryProps {
    gameId : number,
    game : Game,
    history : Event[],
    loadHistory : (gameId: number) => void
    loadGameAndHistory : (gameId: number) => void
}

class loadGameAndHistory extends React.Component<LoadGameAndHistoryProps> {
    componentDidMount() {
        if (!this.props.game ||
            this.props.game.id !== this.props.gameId) {
            this.props.loadGameAndHistory(this.props.gameId);
        } else if (!this.props.history) {
            this.props.loadHistory(this.props.gameId);
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game,
        history : state.activeGame.history
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        loadHistory: (gameId : number) => ThunkActions.loadGameHistory(gameId)(dispatch),
        loadGameAndHistory: (gameId : number) =>
            (ThunkActions.loadGame(gameId)(dispatch))
            .then(_ => ThunkActions.loadGameHistory(gameId)(dispatch))
    };
}

const LoadGameAndHistory = connect(mapStateToProps, mapDispatchToProps)(loadGameAndHistory);

export default LoadGameAndHistory;