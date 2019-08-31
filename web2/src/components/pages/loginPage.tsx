import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import PromptToSignupSection from '../sections/promptToSignupSection';
import { VerticalSpacerLarge } from '../utilities/spacers';
import BasicPageContainer from '../sections/basicPageContainer';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <VerticalSpacerLarge/>
                <LoginForm/>
                <VerticalSpacerLarge/>
                <PromptToSignupSection/>
            </BasicPageContainer>
        );
    }
}