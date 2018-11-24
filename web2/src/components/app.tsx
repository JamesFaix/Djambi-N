import React from 'react';
import { Switch, Route } from 'react-router-dom';
import '../index.css';

import LandingPage from './pages/landingPage';
import LoginPage from './pages/loginPage';
import SignupPage from './pages/signupPage';
import TopMenu from './topMenu';

export default class App extends React.Component {
    render() {
        return (
            <main>
                <TopMenu/>
                <Switch>
                    <Route exact path='/' component={LandingPage}/>
                    <Route path='/signup' component={SignupPage}/>
                    <Route path='/login' component={LoginPage}/>
                </Switch>
            </main>
        );
    }
}