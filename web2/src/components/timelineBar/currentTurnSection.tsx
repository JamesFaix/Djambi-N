import * as React from 'react';
import { TimelineHeader } from '../controls/headers';
import { Game, User, Player, Board } from '../../api/model';
import * as Copy from '../../utilities/copy';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Classes } from '../../styles/styles';
import CurrentTurnActionsBar from './currentTurnActionsBar';
import { Icons } from '../../utilities/icons';
import { Theme } from '../../themes/model';

interface CurrentTurnSectionProps {
    game : Game,
    board : Board,
    user : User,
    theme : Theme
}

class currentTurnSection extends React.Component<CurrentTurnSectionProps> {
    render() {
        const g = this.props.game;
        const u = this.props.user;
        const turn = g ? g.currentTurn : null;

        if (!turn || !u) {
            return null;
        }

        const p = this.getCurrentPlayer(g);
        const isCurrentUser = p.userId === u.id;

        return (
            <div
                id="current-turn-section"
                className={Classes.timelineBarCurrentTurn}
            >
                <TimelineHeader icon={Icons.Timeline.currentTurn(p.name, isCurrentUser)}/>
                <div
                    className={Classes.playerBox}
                    data-player-color-id={p.colorId}
                >
                    {
                        isCurrentUser
                            ? this.renderForCurrentPlayer()
                            : this.renderForOtherPlayer(p)
                    }
                </div>
            </div>
        );
    }

    private getCurrentPlayer(game : Game) {
        return game.players
            .find(p => p.id === game.turnCycle[0]);
    }

    private renderForOtherPlayer(player : Player) {
        return (<>
            {`Waiting on ${player.name}...`}
        </>);
    }

    private renderForCurrentPlayer() {
        const game = this.props.game;
        const board = this.props.board;
        const turn = game.currentTurn;

        return (<>
            {turn.selections.map((s, i) =>
                <p key={"row" + i}>
                    {Copy.getSelectionDescription(this.props.theme, s, game, board)}
                </p>)
            }
            <p style={{
                fontStyle:"italic",
                padding:"5px"
            }}>
                {`(${Copy.getTurnPrompt(turn)})`}
            </p>
            <CurrentTurnActionsBar/>
        </>);
    }
}

const mapStateToProps = (state : State) => {
    const game = state.activeGame.game;
    const param = game ? game.parameters : null;
    const rc = param ? param.regionCount : null;
    const board = rc ? state.boards.boards.get(rc) : null;

    return {
        game: game,
        user: state.session.user,
        board: board,
        theme: state.display.theme
    };
}

const CurrentTurnSection = connect(mapStateToProps)(currentTurnSection);

export default CurrentTurnSection;