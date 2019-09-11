import * as React from 'react';
import { Game } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import Controller from '../../controller';

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
        activeGame: state.activeGame.game,
        loadGame: (gameId : number) => Controller.Game.loadGame(gameId)
    };
};

const LoadGame = connect(mapStateToProps)(loadGame);
export default LoadGame;