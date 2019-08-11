import * as React from 'react';
import { Game } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';

interface PlayPageProps {
    game : Game
}

class playPage extends React.Component<PlayPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;
        return (
            <div>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={{enableDashboard: true, enableLobby: true}}/>
                <LoadGame gameId={gameId}/>
                Game page content
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