import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import PromptToLoginSection from '../sections/promptToLoginSection';
import { VerticalSpacerLarge } from '../utilities/spacers';
import BasicPageContainer from '../sections/basicPageContainer';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableLogin: true}}/>
                <VerticalSpacerLarge/>
                <SignupForm/>
                <VerticalSpacerLarge/>
                <PromptToLoginSection/>
            </BasicPageContainer>
        );
    }
}