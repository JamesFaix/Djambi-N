import * as React from 'react';
import CurrentTurnSection from './currentTurnSection';
import TurnCycleSection from './turnCycleSection';
import GameHistorySection from './gameHistorySection';
import { Classes } from '../../styles/styles';

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