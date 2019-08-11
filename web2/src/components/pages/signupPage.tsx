import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import SignupForm from '../forms/signupForm';

export default class SignupPage extends React.Component<{}>{
    render() {
        return (
            <div>
                <RedirectToDashboardIfLoggedIn/>
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