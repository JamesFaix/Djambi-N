import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { VerticalSpacerLarge } from '../utilities/spacers';
import BasicPageContainer from '../sections/basicPageContainer';

export default class CreateGamePage extends React.Component<{}> {
    render() {
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableDashboard: true}}/>
                <VerticalSpacerLarge/>
                <CreateGameForm/>
            </BasicPageContainer>
        );
    }
}