import * as React from 'react';
import { PlayHeader } from '../controls/headers';
import { Game, User, Player, Turn, TurnStatus, Board } from '../../api/model';
import Colors from '../../utilities/colors';
import * as Copy from '../../utilities/copy';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import Styles from '../../styles/styles';
import CurrentTurnActionsBar from './currentTurnActionsBar';

interface CurrentTurnSectionProps {
    game : Game,
    board : Board,
    user : User
}

class currentTurnSection extends React.Component<CurrentTurnSectionProps> {
    render() {
        const game = this.props.game;
        const turn = game ? game.currentTurn : null;
        if (!turn) {
            return null;
        }

        const player = this.getCurrentPlayer(this.props.game);

        return (
            <div>
                <PlayHeader text="Current turn"/>
                {
                    player.userId === this.props.user.id
                        ? this.renderForCurrentPlayer(player, turn)
                        : this.renderForOtherPlayer(player)
                }
                <CurrentTurnActionsBar/>
            </div>
        );
    }

    private getCurrentPlayer(game : Game) {
        return game.players
            .find(p => p.id === game.turnCycle[0]);
    }

    private renderForOtherPlayer(player : Player) {
        const style = Styles.playerBoxGlow(player.colorId);
        return (
            <div style={style}>
                {`Waiting on ${player.name}...`}
            </div>
        );
    }

    private renderForCurrentPlayer(player : Player, turn : Turn) {
        const style = Styles.playerBoxGlow(player.colorId);
        return (
            <div style={style}>
                <p>
                    {player.name},<br/>
                    {Copy.getTurnPrompt(turn)}
                </p>
                <div>
                    {this.getSelectionsDescription(turn)}
                </div>
            </div>
        );
    }

    private getSelectionsDescription(turn : Turn) {
        const game = this.props.game;
        const board = this.props.board;

        const descriptions = turn.selections
            .map((s, i) => <p key={"row" + i}>{Copy.getSelectionDescription(s, game, board)}</p>);

        return (
            <div>
                Pending turn:
                <br/>
                <div style={{marginLeft: "10px"}}>
                    {descriptions}
                    {this.renderEllipsisIfSelectionRequired(turn)}
                </div>
            </div>
        );
    }

    private renderEllipsisIfSelectionRequired(turn : Turn) {
        return turn.status === TurnStatus.AwaitingSelection
            ? <p>...</p>
            : null;
    }
}

const mapStateToProps = (state : AppState) => {
    const game = state.activeGame.game;
    const param = game ? game.parameters : null;
    const rc = param ? param.regionCount : null;
    const board = rc ? state.boards.boards.get(rc) : null;

    return {
        game : game,
        user : state.session.user,
        board : board
    };
}

const CurrentTurnSection = connect(mapStateToProps)(currentTurnSection);

export default CurrentTurnSection;