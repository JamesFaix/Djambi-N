import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import PromptToLoginSection from '../sections/promptToLoginSection';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <div className={Classes.pageContainer}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableLogin: true}}/>
                <div className={Classes.pageContainerSpacer}></div>
                <SignupForm/>
                <div className={Classes.pageContainerSpacer}></div>
                <PromptToLoginSection/>
            </div>
        );
    }
}