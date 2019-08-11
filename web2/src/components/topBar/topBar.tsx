import * as React from 'react';
import TitleSection from './titleSection';
import UserSection from './userSection';
import NavigationSection from './navigationSection';

const TopBar : React.SFC<{}> = _ => {
    const style = {
        height: "50px",
        borderStyle: "solid",
        borderWidth: "thin",
        borderColor: "gainsboro",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center"
    };

    return (
        <div style={style}>
            <div style={{flex:1, textAlign:"left"}}>
                <NavigationSection/>
            </div>
            <div style={{flex:1, textAlign:"center"}}>
                <TitleSection/>
            </div>
            <div style={{flex:1, textAlign:"right"}}>
                <UserSection/>
            </div>
        </div>
    );
};

export default TopBar;