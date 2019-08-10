import * as React from 'react';
import { Game, GameStatus } from "../../api/model";
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

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

const mapStateToProps = (state: AppState) => {
    return {
        game: state.activeGame.game
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        redirect: (game: Game) => ThunkActions.navigateToGame(game)
    };
};

const RedirectToLobbyIfGameNotInProgress = connect(mapStateToProps, mapDispatchToProps)(redirectToLobbyIfGameNotInProgress);

export default RedirectToLobbyIfGameNotInProgress