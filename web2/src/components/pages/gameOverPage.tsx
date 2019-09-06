import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import { SectionHeader } from '../controls/headers';
import { Game, PlayerStatus, GameStatus } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import RedirectToLobbyIfNotGameStatus from '../utilities/redirectToLobbyIfNotGameStatus';
import LoadGame from '../utilities/loadGame';

interface GameOverPageProps {
    game : Game
}

class gameOverPage extends React.Component<GameOverPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;

        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfNotGameStatus status={GameStatus.Over}/>
                <LoadGame gameId={gameId}/>
                <SectionHeader text="Game over"/>
                <p>
                    {this.getGameOverText()}
                </p>
            </BasicPageContainer>
        );
    }

    private getGameOverText() : string {
        if (!this.props.game
            || this.props.game.status !== GameStatus.Over) {
            return "";
        }

        const ps = this.props.game.players;

        const winners = ps.filter(p => p.status === PlayerStatus.Victorious);
        if (winners.length === 1) {
            return `${winners[0].name} won!`;
        }

        const drawers = ps.filter(p => p.status === PlayerStatus.AcceptsDraw);
        if (drawers.length > 0) {
            const last = drawers.pop();
            let list = drawers.map(p => p.name).join(", ");
            list = list + " and " + last.name;
            return `${list} accepted a draw.`;
        }

        return "Everyone lost. It can only go up from here!";
    }
}

const mapStateToProps = (state : State) => {
    return {
        game : state.activeGame.game
    };
}

const GameOverPage = connect(mapStateToProps)(gameOverPage);
export default GameOverPage;