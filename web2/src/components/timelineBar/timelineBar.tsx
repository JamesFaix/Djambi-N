import * as React from 'react';
import LastTurnSection from './lastTurnSection';
import CurrentTurnSection from './currentTurnSection';
import TurnCycleSection from './turnCycleSection';

const TimelineBar : React.SFC<{}> = _ => {
    const style : React.CSSProperties = {
        height: "150px",
        width: "100%",
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        position: "fixed",
        bottom: 0
    };

    const sectionStyle : React.CSSProperties = {
        flex: 1,
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        height: "100%"
    };

    return (
        <div style={style}>
            <div style={sectionStyle}>
                <LastTurnSection/>
            </div>
            <div style={sectionStyle}>
                <CurrentTurnSection/>
            </div>
            <div style={sectionStyle}>
                <TurnCycleSection/>
            </div>
        </div>
    );
};

export default TimelineBar;