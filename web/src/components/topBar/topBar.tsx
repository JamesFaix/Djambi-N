import * as React from 'react';
import NavigationSection from './topBarNavigationSection';
import { Classes } from '../../styles/styles';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';

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

const TitleSection : React.SFC<{}> = _ => {
    const theme = Selectors.theme();
    return (
        <div
            id={"title-section"}
            className={Classes.topBarTitle}
        >
            <h1>
                {theme.copy.gameTitle}
            </h1>
        </div>
    );
};

//#region User section

const UserSection : React.SFC<{}> = _ => {
    const user = Selectors.user();
    return (
        <div
            id={"user-section"}
            className={Classes.topBarUser}
        >
            {user
                ? <LoggedInUserSection/>
                : <LoggedOutUserSection/>
            }
        </div>
    );
}

const LoggedOutUserSection : React.SFC<{}> = _ => {
    return (<>
        (Not signed in)
    </>);
};

const LoggedInUserSection : React.SFC<{}> = _ => {
    const user = Selectors.user();
    return (<>
        {user.name}
        <IconButton
            icon={Icons.UserActions.logout}
            onClick={() => Controller.Session.logout()}
        />
    </>);
};

//#endregion