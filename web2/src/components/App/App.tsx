import React, {
  FC, useEffect,
} from 'react';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig } from '../../utilities/configService';
import SignupForm from '../forms/SignupForm';
import LoginForm from '../forms/LoginForm';
import UserConfigForm from '../forms/UserConfigForm';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  return (
    <div className="App">
      <NavigationDrawer />
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <UserConfigForm />
        <LoginForm />
        <SignupForm />

      </header>
    </div>
  );
};

export default App;
