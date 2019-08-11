import * as React from 'react';
import { Game } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';

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
                <LoadGame gameId={gameId}/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                <Link to={Routes.lobby(gameId)}>
                    <button>
                        Lobby
                    </button>
                </Link>
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