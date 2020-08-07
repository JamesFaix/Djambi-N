import React, { FC } from 'react';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';

const App: FC = () => {
  return (
    <div className="App">
      <NavigationDrawer />
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
};

export default App;
