import * as React from 'react';
import { Game, GameStatus } from "../../api/model";
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import MiscStoreFlows from '../../storeFlows/misc';

interface RedirectToLobbyIfNotGameStatusProps {
    game: Game,
    status: GameStatus,
    redirect: (game: Game) => void
}

class redirectToLobbyIfNotGameStatus extends React.Component<RedirectToLobbyIfNotGameStatusProps>{
    componentDidMount() {
        const p = this.props;

        if (!p.game) {
            return;
        }

        if (p.game.status !== p.status) {
            p.redirect(p.game);
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state: State) => {
    return {
        game: state.activeGame.game
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        redirect: (game: Game) => MiscStoreFlows.navigateToGame(game)
    };
};

const RedirectToLobbyIfNotGameStatus = connect(mapStateToProps, mapDispatchToProps)(redirectToLobbyIfNotGameStatus);
export default RedirectToLobbyIfNotGameStatus;