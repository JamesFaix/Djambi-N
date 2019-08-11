import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableLogin: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <SignupForm/>
                <div style={Styles.pageContainerSpacer()}></div>
                <div>
                    Already have an account?
                    <Link to={Routes.login}>
                        <button>
                            Log in
                        </button>
                    </Link>
                </div>
            </div>
        );
    }
}