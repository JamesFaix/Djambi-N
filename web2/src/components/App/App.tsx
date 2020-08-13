import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
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
import TopBar from '../TopBar/TopBar';

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
        <TopBar />
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
