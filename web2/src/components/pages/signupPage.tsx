import React from 'react';
import '../../index.css';
import SignupForm from '../forms/signupForm';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';

export default class SignupPage extends React.Component {

    render() {
        const links = [
            { to: '/', label: 'Home' },
            { to: '/login', label: 'Log in' },
        ];

        return (
            <div>
                <PageTitle label="Sign up"/>
                <br/>
                <NavigationStrip links={links}/>
                <br/>
                <br/>
                <SignupForm/>
            </div>
        );
    }
}