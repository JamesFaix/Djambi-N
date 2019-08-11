import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import LoginForm from '../forms/loginForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';

export default class LoginPage extends React.Component<{}>{
    render() {
        return (
            <div>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableSignup: true}}/>
                <LoginForm/>
                <div>
                    Don't have an account yet?
                    <Link to={Routes.signup}>
                        <button>
                            Sign up
                        </button>
                    </Link>
                </div>
            </div>
        );
    }
}