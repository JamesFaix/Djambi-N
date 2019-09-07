import * as React from 'react';
import { Game } from '../../api/model';
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import GameStoreFlows from '../../storeFlows/game';

interface LoadGameProps {
    gameId : number,
    activeGame : Game,
    loadGame : (gameId: number) => void
}

class loadGame extends React.Component<LoadGameProps> {
    componentDidMount() {
        if (!this.props.activeGame ||
            this.props.activeGame.id !== this.props.gameId) {

            this.props.loadGame(this.props.gameId);
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state : State) => {
    return {
        activeGame: state.activeGame.game
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        loadGame: (gameId : number) => GameStoreFlows.loadGame(gameId)(dispatch)
    };
}

const LoadGame = connect(mapStateToProps, mapDispatchToProps)(loadGame);

export default LoadGame;