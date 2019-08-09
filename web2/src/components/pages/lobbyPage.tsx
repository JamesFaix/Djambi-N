import * as React from 'react';
import { Game } from '../../api/model';
import * as Redirects from '../redirects';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Routes from '../../routes';
import { Link } from 'react-router-dom';

interface LobbyPageProps {
    game : Game
}

interface LobbyPageState {

}

class lobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    private getGameJson() {
        if (this.props.game) {
            return JSON.stringify(this.props.game);
        } else {
            return "(no game loaded)";
        }
    }

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
                {this.getGameJson()}
            </div>
        );
    }
}


const mapStateToProps = (state: AppState) => {
    if (state.activeGame) {
        return {
            game: state.activeGame.game
        };
    } else {
        return {
            game: null
        };
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        //Switch from lobby to play
    };
};

const LobbyPage = connect(mapStateToProps, mapDispatchToProps)(lobbyPage);

export default LobbyPage;