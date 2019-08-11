import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

export default class CreateGamePage extends React.Component<{}> {
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableDashboard: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <CreateGameForm/>
            </div>
        );
    }
}