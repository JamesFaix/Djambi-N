import * as React from 'react';
import { GameStatus } from '../../api/model';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameFull from '../utilities/loadGameFull';
import BoardSection from '../sections/boardSection';
import PlayPageContainer from '../sections/playPageContainer';
import RedirectToLobbyIfNotGameStatus from '../utilities/redirectToLobbyIfNotGameStatus';

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