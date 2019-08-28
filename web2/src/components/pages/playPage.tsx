import * as React from 'react';
import { Game } from '../../api/model';
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameFull from '../utilities/loadGameFull';
import BoardSection from '../sections/boardSection';

interface PlayPageProps {
    game : Game
}

class playPage extends React.Component<PlayPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions = {
            enableDashboard: true,
            enableLobby: true,
            enableHistory: true,
            gameId: gameId
        };

        return (
            <div className={Classes.pageContainer}>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGameFull gameId={gameId}/>
                <BoardSection/>
                <TimelineBar/>
            </div>
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {

    };
}

const PlayPage = connect(mapStateToProps, mapDispatchToProps)(playPage);

export default PlayPage;