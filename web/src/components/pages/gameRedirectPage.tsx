import * as React from 'react';
import { GameStatus } from '../../api/model';
import Routes from '../../routes';
import Controller from '../../controllers/controller';

const GameRedirectPage : React.SFC<{}> = props => {
    const routeGameId = Number((props as any).match.params.gameId);

    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn()
        .then(() => Controller.Game.loadGameIfNotLoaded(routeGameId))
        .then(game => {
            const url = getGameUrl(game.id, game.status);
            Controller.navigateTo(url);
        });
    });

    return null;
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