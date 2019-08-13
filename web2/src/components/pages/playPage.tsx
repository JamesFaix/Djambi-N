import * as React from 'react';
import { Game } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameAndHistory from '../utilities/loadGameAndHistory';

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
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGameAndHistory gameId={gameId}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <div>
                    Board
                </div>
                <TimelineBar/>
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
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