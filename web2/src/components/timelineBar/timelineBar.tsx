import * as React from 'react';
import CurrentTurnSection from './currentTurnSection';
import TurnCycleSection from './turnCycleSection';
import GameHistorySection from './gameHistorySection';
import { Classes, Styles } from '../../styles/styles';

const TimelineBar : React.SFC<{}> = _ => {
    return (
        <div
            className={`${Classes.pageContainer} ${Classes.thinBorder}`}
            style={Styles.timelineBar}
        >
            <div
                className={Classes.thinBorder}
                style={Styles.timelineBarSection}
            >
                <TurnCycleSection/>
            </div>
            <div
                className={Classes.thinBorder}
                style={Styles.timelineBarSection}
            >
                <CurrentTurnSection/>
            </div>
            <div
                className={Classes.thinBorder}
                style={Styles.timelineBarHistorySection}
            >
                <GameHistorySection/>
            </div>
        </div>
    );
};

export default TimelineBar;