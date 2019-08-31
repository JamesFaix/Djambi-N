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
import PlayersTable from '../tables/playersTable';
import ApiActions from '../../apiActions';
import { VerticalSpacerLarge } from '../utilities/spacers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';

interface LobbyPageProps {
    game : Game,
    onStartGameClicked: (gameId: number) => void
}

class lobbyPage extends React.Component<LobbyPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const inProgress =  this.props.game && this.props.game.status === GameStatus.InProgress;

        const navOptions = {
            enableDashboard: true,
            enablePlay: inProgress,
            enableDiplomacy: inProgress,
            gameId: gameId
        };

        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGame gameId={gameId}/>
                {this.props.game ? this.renderBody() : null}
            </BasicPageContainer>
        );
    }

    private renderBody() {
        return (<>
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
        </>);
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
            <IconButton
                icon={Icons.UserActions.startGame}
                showTitle={true}
                onClick={() => this.props.onStartGameClicked(this.props.game.id)}
            />
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