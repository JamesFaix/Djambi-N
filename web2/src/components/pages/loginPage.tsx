import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';
import PromptToSignupSection from '../sections/promptToSignupSection';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <LoginForm/>
                <div style={Styles.pageContainerSpacer()}></div>
                <PromptToSignupSection/>
            </div>
        );
    }
}