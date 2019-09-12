import * as React from 'react';
import { Game, GameStatus } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import Controller from '../../controllers/controller';
import Routes from '../../routes';

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
        game: state.activeGame.game,
        redirect: (game: Game) => Controller.navigateTo(Routes.game(game.id))
    };
};

const RedirectToLobbyIfNotGameStatus = connect(mapStateToProps)(redirectToLobbyIfNotGameStatus);
export default RedirectToLobbyIfNotGameStatus;