import * as React from 'react';
import { Game, GameStatus, User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameFull from '../utilities/loadGameFull';
import BoardSection from '../sections/boardSection';
import PlayPageContainer from '../sections/playPageContainer';
import ControllerEffects from '../../controllerEffects';

const playPage : React.SFC<{
    user : User,
    game : Game,
}> = props => {
    const gameId = (props as any).match.params.gameId;

    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    ControllerEffects.redirectToLobbyIfNotGameStatus(props.game, GameStatus.InProgress);

    return (
        <PlayPageContainer>
            <LoadGameFull gameId={gameId}/>
            <BoardSection/>
            <TimelineBar/>
        </PlayPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game,
    };
}

const PlayPage = connect(mapStateToProps)(playPage);
export default PlayPage;