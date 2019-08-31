import * as React from 'react';
import { TimelineHeader } from '../controls/headers';
import { Game, User, Player, Board } from '../../api/model';
import * as Copy from '../../utilities/copy';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Classes } from '../../styles/styles';
import CurrentTurnActionsBar from './currentTurnActionsBar';
import { Icons } from '../../utilities/icons';

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

        const p = this.getCurrentPlayer(this.props.game);
        const isCurrentUser = p.userId === this.props.user.id;

        return (
            <div>
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
                    {Copy.getSelectionDescription(s, game, board)}
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
        game : game,
        user : state.session.user,
        board : board
    };
}

const CurrentTurnSection = connect(mapStateToProps)(currentTurnSection);

export default CurrentTurnSection;