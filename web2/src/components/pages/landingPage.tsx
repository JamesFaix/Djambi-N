import React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';

export default class LandingPage extends React.Component {

    render() {
        const links = [
            { to: '/login', label: 'Login' },
            { to: '/signup', label: 'Sign up' },
        ];

        return (
            <div>
                <PageTitle label="Welcome"/>
                <br/>
                <NavigationStrip links={links}/>
            </div>
        );
    }
}