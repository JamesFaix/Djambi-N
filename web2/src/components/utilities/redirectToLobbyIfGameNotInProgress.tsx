import * as React from 'react';
import { Game, GameStatus } from "../../api/model";
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import MiscStoreFlows from '../../storeFlows/misc';

interface RedirectToLobbyIfGameNotInProgressProps {
    game: Game,
    redirect: (game: Game) => void
}

class redirectToLobbyIfGameNotInProgress extends React.Component<RedirectToLobbyIfGameNotInProgressProps>{
    componentDidMount() {
        if (!this.props.game) {
            return;
        }

        if (this.props.game.status !== GameStatus.InProgress) {
            this.props.redirect(this.props.game);
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

const RedirectToLobbyIfGameNotInProgress = connect(mapStateToProps, mapDispatchToProps)(redirectToLobbyIfGameNotInProgress);

export default RedirectToLobbyIfGameNotInProgress