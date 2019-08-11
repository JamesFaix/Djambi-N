import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <div>
                <RedirectToDashboardIfLoggedIn/>
                <SetNavigationOptions options={{enableLogin: true}}/>
                <SignupForm/>
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