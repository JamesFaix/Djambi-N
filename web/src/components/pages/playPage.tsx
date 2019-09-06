import * as React from 'react';
import { Game, GameStatus } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameFull from '../utilities/loadGameFull';
import BoardSection from '../sections/boardSection';
import PlayPageContainer from '../sections/playPageContainer';
import RedirectToLobbyIfNotGameStatus from '../utilities/redirectToLobbyIfNotGameStatus';

interface PlayPageProps {
    game : Game,
}

class playPage extends React.Component<PlayPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;
        return (
            <PlayPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfNotGameStatus status={GameStatus.InProgress}/>
                <LoadGameFull gameId={gameId}/>
                <BoardSection/>
                <TimelineBar/>
            </PlayPageContainer>
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game,
    };
}

const PlayPage = connect(mapStateToProps)(playPage);

export default PlayPage;