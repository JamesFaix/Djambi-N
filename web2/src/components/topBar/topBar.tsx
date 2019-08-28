import * as React from 'react';
import TitleSection from './titleSection';
import UserSection from './userSection';
import NavigationSection from './navigationSection';
import { Classes, Styles } from '../../styles/styles';

const TopBar : React.SFC<{}> = _ => {
    return (
        <div
            className={Classes.thinBorder}
            style={Styles.topBar}
        >
            <div style={Styles.topBarNavigation}>
                <NavigationSection/>
            </div>
            <div style={Styles.topBarTitle}>
                <TitleSection/>
            </div>
            <div style={Styles.topBarUser}>
                <UserSection/>
            </div>
        </div>
    );
};

export default TopBar;