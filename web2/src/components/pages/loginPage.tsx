import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <LoginForm/>
                <div style={Styles.pageContainerSpacer()}></div>
                <div>
                    Don't have an account yet?
                    <Link to={Routes.signup}>
                        <button
                            style={{margin:"10px"}}
                        >
                            Sign up
                        </button>
                    </Link>
                </div>
            </div>
        );
    }
}