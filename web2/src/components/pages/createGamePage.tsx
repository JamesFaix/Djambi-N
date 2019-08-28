import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import { VerticalSpacerLarge } from '../utilities/spacers';

export default class CreateGamePage extends React.Component<{}> {
    render() {
        return (
            <div className={Classes.pageContainer}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableDashboard: true}}/>
                <VerticalSpacerLarge/>
                <CreateGameForm/>
            </div>
        );
    }
}