import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import PromptToLoginSection from '../sections/promptToLoginSection';
import BasicPageContainer from '../sections/basicPageContainer';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToDashboardIfLoggedIn/>
                <SignupForm/>
                <br/>
                <PromptToLoginSection/>
            </BasicPageContainer>
        );
    }
}