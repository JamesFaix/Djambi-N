import * as React from 'react';
import { Game, GameStatus } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import Routes from '../../routes';
import * as Redirects from '../redirects';
import { Link } from 'react-router-dom';

interface PlayPageProps {
    game : Game
}

class playPage extends React.Component<PlayPageProps> {
    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <Redirects.ToHomeIfNoGame/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                <Link to={Routes.lobby(this.props.game.id)}>
                    <button>
                        Lobby
                    </button>
                </Link>
                {this.redirectIfNotInProgress()}
                Game page content
            </div>
        );
    }

    private redirectIfNotInProgress(){
        if (this.props.game.status === GameStatus.InProgress) {
            return null;
        } else {
            return <Redirect to={Routes.lobby(this.props.game.id)}/>;
        }
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