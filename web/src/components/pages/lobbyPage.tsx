import * as React from 'react';
import GameParametersTable from '../tables/gameParametersTable';
import MutablePlayersTable from '../tables/mutablePlayersTable';
import { Game, GameStatus, User } from '../../api/model';
import { connect } from 'react-redux';
import { State } from '../../store/root';
import LoadGame from '../utilities/loadGame';
import PlayersTable from '../tables/playersTable';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';
import Controller from '../../controller';
import ControllerEffects from '../../controllerEffects';

const lobbyPage : React.SFC<{
    user : User,
    game : Game,
    onStartGameClicked: (gameId: number) => void
}> = props => {
    const gameId = (props as any).match.params.gameId;
    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    return (
        <BasicPageContainer>
            <LoadGame gameId={gameId}/>
            <PageBody
                game={props.game}
                user={props.user}
                startGame={gameId => props.onStartGameClicked(gameId)}
            />
        </BasicPageContainer>
    );
}

const PageBody : React.SFC<{
    game : Game,
    user : User,
    startGame : (gameId : number) => void
}> = props => {
    if (!props.game) { return null; }

    return (<>
        <GameParametersTable/>
        <br/>
        <br/>
        {
            props.game.status === GameStatus.Pending
                ? <MutablePlayersTable/>
                : <PlayersTable/>
        }
        <br/>
        <br/>
        <StartButton
            game={props.game}
            user={props.user}
            startGame={gameId => props.startGame(gameId)}
        />
    </>);
}

const StartButton : React.SFC<{
    game : Game,
    user : User,
    startGame : (gameId : number) => void
}> = props => {
    const g = props.game;
    const u = props.user;

    const canStart = g && u &&
        g.createdBy.userId === u.id &&
        g.status === GameStatus.Pending &&
        g.players.length > 1;

    return canStart ?
        <IconButton
            icon={Icons.UserActions.startGame}
            showTitle={true}
            onClick={() => props.startGame(g.id)}
        />
    : null;
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