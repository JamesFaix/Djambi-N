import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import { GameStatus, Game } from '../../api/model';
import LoadGame from '../utilities/loadGame';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import Routes from '../../routes';
import Controller from '../../controller';

const gameRedirectPage : React.SFC<{
    game : Game
}> = props => {
    const routeGameId = Number((props as any).match.params.gameId);

    React.useEffect(() => {
        if (props.game && props.game.id === routeGameId) {
            const url = getUrl(routeGameId, props.game.status);
            Controller.navigateTo(url);
        }
    });

    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <LoadGame gameId={routeGameId}/>
    </BasicPageContainer>
    );
}

function getUrl(gameId : number, status : GameStatus) : string {
    switch (status) {
        case GameStatus.Canceled:
        case GameStatus.Pending:
            return Routes.lobby(gameId);
        case GameStatus.Over:
            return Routes.gameOver(gameId);
        case GameStatus.InProgress:
            return Routes.play(gameId);
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
