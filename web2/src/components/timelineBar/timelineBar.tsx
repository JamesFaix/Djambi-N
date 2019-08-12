import * as React from 'react';
import CurrentTurnSection from './currentTurnSection';
import TurnCycleSection from './turnCycleSection';
import GameHistorySection from './gameHistorySection';

const TimelineBar : React.SFC<{}> = _ => {
    const style : React.CSSProperties = {
        height: "100%",
        width: "400px",
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
        alignItems: "center",
        position: "fixed",
        right: 0
    };

    const sectionStyle : React.CSSProperties = {
        flex: 1,
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        width: "100%"
    };

    return (
        <div style={style}>
            <div style={sectionStyle}>
                <TurnCycleSection/>
            </div>
            <div style={sectionStyle}>
                <CurrentTurnSection/>
            </div>
            <div style={sectionStyle}>
                <GameHistorySection/>
            </div>
        </div>
    );
};

export default TimelineBar;