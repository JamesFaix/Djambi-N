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
import SignupForm from '../forms/SignupForm';
import LoginForm from '../forms/LoginForm';
import UserConfigForm from '../forms/UserConfigForm';
import * as Routes from '../../utilities/routes';

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
            <Route path={Routes.settings}>
              <UserConfigForm />
            </Route>
            <Route path={Routes.signIn}>
              <LoginForm />
            </Route>
            <Route path={Routes.createAccount}>
              <SignupForm />
            </Route>
          </Switch>
        </div>
      </BrowserRouter>
    </div>
  );
};

export default App;
