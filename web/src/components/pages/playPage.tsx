import * as React from 'react';
import { GameStatus } from '../../api/model';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import PlayPageContainer from '../containers/playPageContainer';
import RedirectToLobbyIfNotGameStatus from '../utilities/redirectToLobbyIfNotGameStatus';
import { BoardSection } from './playPageBoardSection';
import LoadGameFull from '../utilities/loadGameFull';
import TimelineBar from './playPageTimelineSection';

const PlayPage : React.SFC<{}> = props => {
    const routeGameId = (props as any).match.params.gameId;
    return (
        <PlayPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <RedirectToLobbyIfNotGameStatus status={GameStatus.InProgress}/>
            <LoadGameFull gameId={routeGameId}/>
            <BoardSection/>
            <TimelineBar/>
        </PlayPageContainer>
    );
}
export default PlayPage;