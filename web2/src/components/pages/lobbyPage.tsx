import * as React from 'react';
import * as Redirects from '../redirects';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import GameParametersTable from '../lobby/gameParametersTable';
import LobbyPlayersTable from '../lobby/lobbyPlayersTable';
import { Game, GameStatus } from '../../api/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import { AppState } from '../../store/state';

interface LobbyPageProps {
    game : Game,
    onStartGameClicked: (gameId: number) => void
}

class lobbyPage extends React.Component<LobbyPageProps> {
    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <Redirects.ToHomeIfNoGame/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                {this.renderPlayButton()}
                {this.renderStartButton()}
                <GameParametersTable/>
                <LobbyPlayersTable/>
            </div>
        );
    }

    private renderStartButton() {
        if (this.props.game.status !== GameStatus.Pending
            || this.props.game.players.length < 2) {
            return null;
        }

        return (
            <button
                onClick={() => this.props.onStartGameClicked(this.props.game.id)}
            >
                Start
            </button>
        );
    }

    private renderPlayButton() {
        if (this.props.game.status !== GameStatus.InProgress) {
            return null;
        }

        return (
            <Link to={Routes.play(this.props.game.id)}>
                <button>
                    Play
                </button>
            </Link>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onStartGameClicked: (gameId : number) => ThunkActions.startGame(gameId)(dispatch)
    };
}

const LobbyPage = connect(mapStateToProps, mapDispatchToProps)(lobbyPage);

export default LobbyPage;