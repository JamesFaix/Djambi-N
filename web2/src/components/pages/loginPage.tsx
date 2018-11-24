import * as React from 'react';
import '../../index.css';
import LoginForm from '../forms/loginForm';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';

export default class LoginPage extends React.Component {

    render() {
        const links = [
            { to: '/', label: 'Home' },
            { to: '/signup', label: 'Sign up' },
        ];

        return (
            <div>
                <PageTitle label="Log in"/>
                <br/>
                <NavigationStrip links={links}/>
                <br/>
                <br/>
                <LoginForm/>
            </div>
        );
    }
}