import * as React from 'react';
import GameParametersTable from '../tables/gameParametersTable';
import MutablePlayersTable from '../tables/mutablePlayersTable';
import { Game, GameStatus } from '../../api/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { State } from '../../store/root';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import PlayersTable from '../tables/playersTable';
import ApiActions from '../../apiActions';
import { VerticalSpacerLarge } from '../utilities/spacers';

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
            <div className={Classes.pageContainer}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGame gameId={gameId}/>
                {this.props.game ? this.renderBody() : null}
            </div>
        );
    }

    private renderBody() {
        return (
            <React.Fragment>
                <VerticalSpacerLarge/>
                <GameParametersTable/>
                <VerticalSpacerLarge/>
                {
                    this.props.game.status === GameStatus.Pending
                        ? <MutablePlayersTable/>
                        : <PlayersTable/>
                }
                <VerticalSpacerLarge/>
                {this.renderStartButton()}
            </React.Fragment>
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

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onStartGameClicked: (gameId : number) => ApiActions.startGame(gameId)(dispatch)
    };
}

const LobbyPage = connect(mapStateToProps, mapDispatchToProps)(lobbyPage);

export default LobbyPage;