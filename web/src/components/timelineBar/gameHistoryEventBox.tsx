import * as React from 'react';
import { Event, Game, Board } from "../../api/model";
import GameHistoryEffectBox from './gameHistoryEffectBox';
import { Classes } from '../../styles/styles';
import GameHistory from '../../viewModel/gameHistory';
import Copy from '../../utilities/copy';
import { dateToString } from '../../utilities/dates';

interface GameHistoryEventBoxProps {
    game : Game, //This will be needed eventually to get the correct player names from playerIDs in event objects
    event : Event,
    board : Board
}

const GameHistoryEventBox : React.SFC<GameHistoryEventBoxProps> = props => {
    const e = props.event;
    if (!e || !props.game || !props.board) {
        return null;
    }

    const p = props.game.players.find(p => p.id === e.actingPlayerId);

    const dateText = dateToString(e.createdBy.time);

    return (
        <div
            className={p ? Classes.playerBox : Classes.box}
            data-player-color-id={p ? p.colorId : null}
        >
            <div style={{ display: "flex" }}>
                <div style={{ flex: 1 }}>
                    {Copy.getEventDescription(e, props.game)}
                </div>
                <div style={{ textAlign: "right" }}>
                    {dateText}
                </div>
            </div>
            <div className={Classes.indented}>
                {e.effects
                    .filter(f => GameHistory.isDisplayableEffect(f))
                    .map((f, i) => {
                    return (
                        <GameHistoryEffectBox
                            game={props.game}
                            effect={f}
                            key={i}
                            board={props.board}
                        />
                    );
                })}
            </div>
        </div>
    );
}

export default GameHistoryEventBox;