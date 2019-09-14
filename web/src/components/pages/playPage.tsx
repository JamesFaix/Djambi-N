import * as React from 'react';
import { GameStatus } from '../../api/model';
import PlayPageContainer from '../containers/playPageContainer';
import { BoardSection } from '../pageSections/playPageBoardSection';
import TimelineBar from '../pageSections/playPageTimelineSection';
import Controller from '../../controllers/controller';

const PlayPage : React.SFC<{}> = props => {
    const routeGameId = (props as any).match.params.gameId;

    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn()
        .then(() => Controller.Game.loadGameFull(routeGameId))
        .then(() => Controller.Game.redirectToLobbyIfGameNotStatus(GameStatus.InProgress));
    });
    return (
        <PlayPageContainer>
            <BoardSection/>
            <TimelineBar/>
        </PlayPageContainer>
    );
}
export default PlayPage;