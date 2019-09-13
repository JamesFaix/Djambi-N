import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import { GameStatus, Game } from '../../api/model';
import LoadGame from '../utilities/loadGame';
import Routes from '../../routes';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';

const GameRedirectPage : React.SFC<{}> = props => {
    const routeGameId = Number((props as any).match.params.gameId);
    const game = Selectors.game();

    React.useEffect(() => {
        if (game && game.id === routeGameId) {
            const url = getGameUrl(routeGameId, game.status);
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
export default GameRedirectPage;

function getGameUrl(gameId : number, status : GameStatus) : string {
    switch (status) {
        case GameStatus.Canceled:
        case GameStatus.Pending:
            return Routes.lobby(gameId);
        case GameStatus.Over:
            return Routes.gameResults(gameId);
        case GameStatus.InProgress:
            return Routes.play(gameId);
        default:
            return null;
    }
}