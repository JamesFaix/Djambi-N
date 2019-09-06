import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import PromptToSignupSection from '../sections/promptToSignupSection';
import BasicPageContainer from '../sections/basicPageContainer';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToDashboardIfLoggedIn/>
                <LoginForm/>
                <br/>
                <PromptToSignupSection/>
            </BasicPageContainer>
        );
    }
}