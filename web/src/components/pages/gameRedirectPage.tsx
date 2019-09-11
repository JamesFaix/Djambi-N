import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import { GameStatus, Game } from '../../api/model';
import LoadGame from '../utilities/loadGame';
import LobbyPage from './lobbyPage';
import GameOverPage from './gameOverPage';
import PlayPage from './playPage';
import { State } from '../../store/root';
import { connect } from 'react-redux';

const gameRedirectPage : React.SFC<{
    game : Game
}> = props => {
    const gameId = (props as any).match.params.gameId;
    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <LoadGame gameId={gameId}/>
            <PageSwitch
                status={props.game ? props.game.status : null}
            />
    </BasicPageContainer>
    );
}

const PageSwitch : React.SFC<{
    status : GameStatus
}> = props => {
    switch (props.status) {
        case GameStatus.Canceled:
        case GameStatus.Pending:
            return <LobbyPage/>;
        case GameStatus.Over:
            return <GameOverPage/>;
        case GameStatus.InProgress:
            return <PlayPage/>;
        default:
            return null;
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game
    };
}

const GameRedirectPage = connect(mapStateToProps)(gameRedirectPage);
export default GameRedirectPage;
