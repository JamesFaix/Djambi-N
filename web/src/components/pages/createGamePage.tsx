import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';
import BasicPageContainer from '../sections/basicPageContainer';

export default class CreateGamePage extends React.Component<{}> {
    render() {
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <CreateGameForm/>
            </BasicPageContainer>
        );
    }
}