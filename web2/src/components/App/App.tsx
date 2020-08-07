import React, { FC, useEffect, useState } from 'react';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { getConfig } from '../../utilities/configService';
import { defaultConfig } from '../../model/configuration';

const App: FC = () => {
  const [config, setConfig] = useState(defaultConfig);

  useEffect(() => {
    getConfig()
      .then((c) => setConfig(c));
  });

  return (
    <div className="App">
      <NavigationDrawer />
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <br />
        API URL is {config.environment.apiUrl}
        <br />
        Favorite word is {config.user.favoriteWord}
      </header>
    </div>
  );
};

export default App;
