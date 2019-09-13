import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../containers/basicPageContainer';
import { SectionHeader } from '../controls/headers';
import { Game, PlayerStatus, GameStatus } from '../../api/model';
import RedirectToLobbyIfNotGameStatus from '../utilities/redirectToLobbyIfNotGameStatus';
import LoadGame from '../utilities/loadGame';
import Selectors from '../../selectors';

const GameResultsPage : React.SFC<{}> = props => {
    const gameId = (props as any).match.params.gameId;
    const game = Selectors.game();
    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <RedirectToLobbyIfNotGameStatus status={GameStatus.Over}/>
            <LoadGame gameId={gameId}/>
            <SectionHeader text="Game over"/>
            <p>{getGameOverText(game)}</p>
        </BasicPageContainer>
    );
}
export default GameResultsPage;

function getGameOverText(game : Game) : string {
    if (!game || game.status !== GameStatus.Over) {
        return "";
    }

    const winners = game.players.filter(p => p.status === PlayerStatus.Victorious);
    if (winners.length === 1) {
        return `${winners[0].name} won!`;
    }

    const drawers = game.players.filter(p => p.status === PlayerStatus.AcceptsDraw);
    if (drawers.length > 0) {
        const last = drawers.pop();
        let list = drawers.map(p => p.name).join(", ");
        list = list + " and " + last.name;
        return `${list} accepted a draw.`;
    }

    return "Everyone lost. It can only go up from here!";
}