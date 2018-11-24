import React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';

export default class LandingPage extends React.Component {

    render() {
        const links = [
            { to: '/signup', label: 'Sign up' },
            { to: '/login', label: 'Login' },
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