import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import PromptToSignupSection from '../sections/promptToSignupSection';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <div className={Classes.pageContainer}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <div className={Classes.pageContainerSpacer}></div>
                <LoginForm/>
                <div className={Classes.pageContainerSpacer}></div>
                <PromptToSignupSection/>
            </div>
        );
    }
}