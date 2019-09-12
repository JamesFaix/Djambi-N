import * as React from 'react';
import GameParametersTable from '../tables/gameParametersTable';
import MutablePlayersTable from '../tables/mutablePlayersTable';
import { Game, GameStatus, User } from '../../api/model';
import { connect } from 'react-redux';
import { State } from '../../store/root';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import PlayersTable from '../tables/playersTable';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';
import Controller from '../../controllers/controller';

interface LobbyPageProps {
    user : User,
    game : Game,
    onStartGameClicked: (gameId: number) => void
}

class lobbyPage extends React.Component<LobbyPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;
        const g = this.props.game;
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <LoadGame gameId={gameId}/>
                {g ? this.renderBody() : null}
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
        user: state.session.user,
        onStartGameClicked: (gameId : number) => Controller.Game.startGame(gameId)
    };
};

const LobbyPage = connect(mapStateToProps)(lobbyPage);
export default LobbyPage;