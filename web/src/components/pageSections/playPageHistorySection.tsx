import * as React from 'react';
import { Event, Effect } from "../../api/model";
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import { TimelineHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import { Icons } from '../../utilities/icons';
import Scrollbars from 'react-custom-scrollbars';
import Selectors from '../../selectors';
import GameHistory from '../../viewModel/gameHistory';
import Copy from '../../utilities/copy';
import DateService from '../../utilities/dates';

const GameHistorySection : React.SFC<{}> = _ => {
    const history = useSelector((state : AppState) => state.activeGame.history);
    if (!history) { return null; }
    return (
        <div
            id="history-section"
            className={Classes.timelineBarHistory}
        >
            <TimelineHeader icon={Icons.Timeline.history}/>
            <Scrollbars>
                {history.map((e, i) => <GameHistoryEventBox event={e} key={i}/>)}
            </Scrollbars>
        </div>
    );
}
export default GameHistorySection;

const GameHistoryEventBox : React.SFC<{ event : Event }> = props => {
    const e = props.event;
    const game = Selectors.game();
    const board = Selectors.board(game.parameters.regionCount);
    if (!e || !game || !board) { return null; }

    const p = game.players.find(p => p.id === e.actingPlayerId);

    const dateText = DateService.dateToString(e.createdBy.time);

    return (
        <div
            className={p ? Classes.playerBox : Classes.box}
            data-player-color-id={p ? p.colorId : null}
        >
            <div style={{ display: "flex" }}>
                <div style={{ flex: 1 }}>
                    {Copy.getEventDescription(e, game)}
                </div>
                <div style={{ textAlign: "right" }}>
                    {dateText}
                </div>
            </div>
            <div className={Classes.indented}>
                {e.effects
                    .filter(f => GameHistory.isDisplayableEffect(f))
                    .map((f, i) => <GameHistoryEffectBox effect={f} key={i}/>)
                }
            </div>
        </div>
    );
}

const GameHistoryEffectBox : React.SFC<{ effect : Effect }> = props => {
    const game = Selectors.game();
    const board = Selectors.board(game.parameters.regionCount);
    return (
        <div>
            {Copy.getEffectDescription(props.effect, game, board)}
        </div>
    );
}