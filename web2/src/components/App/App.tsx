import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig } from '../../utilities/configService';
import CreateAccountForm from '../forms/CreateAccountForm';
import SignInForm from '../forms/SignInForm';
import UserConfigForm from '../forms/UserConfigForm';
import * as Routes from '../../utilities/routes';
import NoMatchPage from '../pages/NoMatchPage';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  return (
    <div className="App">
      <BrowserRouter>
        <NavigationDrawer />
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" style={{ width: '200px' }} />
        </header>
        <div style={{ padding: '20px' }}>
          <Switch>
            <Route path={Routes.settings} component={UserConfigForm} />
            <Route path={Routes.signIn} component={SignInForm} />
            <Route path={Routes.createAccount} component={CreateAccountForm} />
            <Route component={NoMatchPage} />
          </Switch>
        </div>
      </BrowserRouter>
    </div>
  );
};

export default App;
