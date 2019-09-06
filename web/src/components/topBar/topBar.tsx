import * as React from 'react';
import TitleSection from './titleSection';
import UserSection from './userSection';
import NavigationSection from './navigationSection';
import { Classes } from '../../styles/styles';

const TopBar : React.SFC<{}> = _ => {
    return (
        <div
            id={"top-bar"}
            className={`${Classes.topBar}`}
        >
            <NavigationSection/>
            <TitleSection/>
            <UserSection/>
        </div>
    );
};

export default TopBar;