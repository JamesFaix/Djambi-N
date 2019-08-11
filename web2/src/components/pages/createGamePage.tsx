import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';

export default class CreateGamePage extends React.Component<{}> {
    render() {
        return (
            <div>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableDashboard: true}}/>
                <CreateGameForm/>
            </div>
        );
    }
}