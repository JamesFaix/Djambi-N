import * as React from 'react';
import { Classes } from '../../styles/styles';
import { TimelineHeader } from '../controls/headers';
import { Icons } from '../../utilities/icons';
import Selectors from '../../selectors';
import { Player } from '../../api/model';
import CurrentTurnSection from '../pages/playPageCurrentTurnSection';
import GameHistorySection from './playPageHistorySection';

const TimelineBar : React.SFC<{}> = _ => {
    return (
        <div
            id="timeline-bar"
            className={Classes.timelineBar}
        >
            <TurnCycleSection/>
            <CurrentTurnSection/>
            <GameHistorySection/>
        </div>
    );
};
export default TimelineBar;

//#region Turn cycle

const TurnCycleSection : React.SFC<{}> = _ => {
    const game = Selectors.game();
    if (!game) { return null; }

    const players = game.turnCycle.map(pId =>
        game.players.find(p => p.id === pId));

    return (
        <div
            id="turn-cycle-secion"
            className={Classes.timelineBarTurnCycle}
        >
            <TimelineHeader icon={Icons.Timeline.turnCycle}/>
            <div style={{display:"flex"}}>
                {players.map((p, i) => {
                    return <TurnCycleTurnBox player={p} key={i}/>;
                })}
            </div>
        </div>
    );
}

const TurnCycleTurnBox : React.SFC<{
    player : Player
}> = props => {
    return (
        <div
            className={Classes.playerBox}
            data-player-color-id={props.player.colorId}
            style={{flex:1,textAlign:"center"}}
        >
            {props.player.name}
        </div>
    );
}

//#endregion