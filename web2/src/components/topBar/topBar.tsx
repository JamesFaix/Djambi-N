import * as React from 'react';
import TitleSection from './titleSection';
import UserSection from './userSection';

const TopBar : React.SFC<{}> = _ => {
    const style = {
        height: "50px",
        borderStyle: "solid",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center"
    };

    return (
        <div style={style}>
            <TitleSection/>
            <UserSection/>
        </div>
    );
};

export default TopBar;