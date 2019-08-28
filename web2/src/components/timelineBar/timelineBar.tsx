import * as React from 'react';
import CurrentTurnSection from './currentTurnSection';
import TurnCycleSection from './turnCycleSection';
import GameHistorySection from './gameHistorySection';
import { Classes } from '../../styles/styles';

const TimelineBar : React.SFC<{}> = _ => {
    const style : React.CSSProperties = {
        height: "100%",
        width: "400px",
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        justifyContent: "space-between",
        position: "fixed",
        right: 0
    };

    const sectionStyle : React.CSSProperties = {
        flex: 0,
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        width: "100%"
    };

    const historySectionStyle : React.CSSProperties = {
        flex: 1,
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        width: "100%"
    };

    return (
        <div
            className={Classes.pageContainer}
            style={style}
        >
            <div style={sectionStyle}>
                <TurnCycleSection/>
            </div>
            <div style={sectionStyle}>
                <CurrentTurnSection/>
            </div>
            <div style={historySectionStyle}>
                <GameHistorySection/>
            </div>
        </div>
    );
};

export default TimelineBar;