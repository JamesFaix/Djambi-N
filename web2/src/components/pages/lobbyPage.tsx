import * as React from 'react';
import GameParametersTable from '../tables/gameParametersTable';
import LobbyPlayersTable from '../tables/lobbyPlayersTable';
import { Game, GameStatus } from '../../api/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import { AppState } from '../../store/state';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

interface LobbyPageProps {
    game : Game,
    onStartGameClicked: (gameId: number) => void
}

class lobbyPage extends React.Component<LobbyPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions = {
            enableDashboard: true,
            enableHistory: true,
            enablePlay: this.props.game && this.props.game.status === GameStatus.InProgress,
            gameId: gameId
        };

        return (
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGame gameId={gameId}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <GameParametersTable/>
                <div style={Styles.pageContainerSpacer()}></div>
                <LobbyPlayersTable/>
                <div style={Styles.pageContainerSpacer()}></div>
                {this.renderStartButton()}
            </div>
        );
    }

    private renderStartButton() {
        if (!this.props.game) {
            return null;
        }

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