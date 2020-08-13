import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig } from '../../utilities/config';
import * as Routes from '../../utilities/routes';
import NoMatchPage from '../pages/NoMatchPage';
import RedirectBasedOnStore from '../routing/RedirectBasedOnStore';
import CreateAccountPage from '../pages/CreateAccountPage';
import SignInPage from '../pages/SignInPage';
import UserConfigPage from '../pages/UserConfigPage';
import { restoreSession } from '../../controllers/userController';

const App: FC = () => {
  useEffect(() => {
    loadConfig()
      .then(() => restoreSession()); // Must happen after environment config is loaded
  }, []);

  return (
    <div className="App">
      <BrowserRouter>
        <RedirectBasedOnStore />
        <NavigationDrawer />
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" style={{ width: '200px' }} />
        </header>
        <div style={{ padding: '20px' }}>
          <Switch>
            <Route path={Routes.settings} component={UserConfigPage} />
            <Route path={Routes.signIn} component={SignInPage} />
            <Route path={Routes.createAccount} component={CreateAccountPage} />
            <Route component={NoMatchPage} />
          </Switch>
        </div>
      </BrowserRouter>
    </div>
  );
};

export default App;
