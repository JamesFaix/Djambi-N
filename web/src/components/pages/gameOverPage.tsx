import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import { SectionHeader } from '../controls/headers';
import { Game, PlayerStatus, GameStatus, User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import LoadGame from '../utilities/loadGame';
import ControllerEffects from '../../controllerEffects';

const gameOverPage : React.SFC<{
    user : User,
    game : Game
}> = props => {
    const gameId = (props as any).match.params.gameId;

    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    ControllerEffects.redirectToLobbyIfNotGameStatus(props.game, GameStatus.Over);

    return (
        <BasicPageContainer>
            <LoadGame gameId={gameId}/>
            <SectionHeader text="Game over"/>
            <p>
                {getGameOverText(props.game)}
            </p>
        </BasicPageContainer>
    );
}

function getGameOverText(game : Game) : string {
    if (!game
        || game.status !== GameStatus.Over) {
        return "";
    }

    const winners = game.players.filter(p => p.status === PlayerStatus.Victorious);
    if (winners.length === 1) {
        return `${winners[0].name} won!`;
    }

    const drawers = game.players.filter(p => p.status === PlayerStatus.AcceptsDraw);
    if (drawers.length > 0) {
        const last = drawers.pop();
        let list = drawers.map(p => p.name).join(", ");
        list = list + " and " + last.name;
        return `${list} accepted a draw.`;
    }

    return "Everyone lost. It can only go up from here!";
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user,
        game : state.activeGame.game
    };
}

const GameOverPage = connect(mapStateToProps)(gameOverPage);
export default GameOverPage;