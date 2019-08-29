import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import PromptToSignupSection from '../sections/promptToSignupSection';
import { VerticalSpacerLarge } from '../utilities/spacers';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <div className={Classes.pageContainer}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <VerticalSpacerLarge/>
                <LoginForm/>
                <VerticalSpacerLarge/>
                <PromptToSignupSection/>
            </div>
        );
    }
}