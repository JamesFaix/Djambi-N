import * as React from 'react';
import GameParametersTable from '../tables/gameParametersTable';
import MutablePlayersTable from '../tables/mutablePlayersTable';
import { Game, GameStatus, User } from '../../api/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { State } from '../../store/root';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import PlayersTable from '../tables/playersTable';
import ApiActions from '../../apiActions';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';

interface LobbyPageProps {
    user : User,
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
            <GameParametersTable/>
            <br/>
            <br/>
            {
                this.props.game.status === GameStatus.Pending
                    ? <MutablePlayersTable/>
                    : <PlayersTable/>
            }
            <br/>
            <br/>
            {this.renderStartButton()}
        </>);
    }

    private renderStartButton() {
        const g = this.props.game;
        const u = this.props.user;

        const canStart = g && u &&
            g.createdBy.userId === u.id &&
            g.status === GameStatus.Pending &&
            g.players.length > 1;

        return canStart ?
            <IconButton
                icon={Icons.UserActions.startGame}
                showTitle={true}
                onClick={() => this.props.onStartGameClicked(g.id)}
            />
        : null;
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game,
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onStartGameClicked: (gameId : number) => ApiActions.startGame(gameId)(dispatch)
    };
}

const LobbyPage = connect(mapStateToProps, mapDispatchToProps)(lobbyPage);

export default LobbyPage;