import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';
import PromptToLoginSection from '../sections/promptToLoginSection';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableLogin: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <SignupForm/>
                <div style={Styles.pageContainerSpacer()}></div>
                <PromptToLoginSection/>
            </div>
        );
    }
}