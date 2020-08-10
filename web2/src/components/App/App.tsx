import React, { FC, useEffect, ChangeEvent } from 'react';
import { useSelector } from 'react-redux';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig, setUserConfig } from '../../utilities/configService';
import { RootState } from '../../redux/root';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  const selectConfig = (state: RootState) => state.config;
  const config = useSelector(selectConfig);

  const onLogReduxChanged = (e: ChangeEvent<HTMLInputElement>) => {
    const isChecked = e.target.checked;
    const newConfig = {
      ...config.user,
      logRedux: isChecked,
    };
    setUserConfig(newConfig);
  };

  const onFavoriteWordChanged = (e: ChangeEvent<HTMLInputElement>) => {
    const word = e.target.value;
    const newConfig = {
      ...config.user,
      favoriteWord: word,
    };
    setUserConfig(newConfig);
  };

  return (
    <div className="App">
      <NavigationDrawer />
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <br />
        <p>
          API URL is {config.environment.apiUrl}
        </p>
        <br />
        <p>
          Favorite word is
          <input
            type="text"
            value={config.user.favoriteWord}
            onChange={onFavoriteWordChanged}
          />
        </p>
        <br />
        <p>
          Log redux:
          <input
            type="checkbox"
            checked={config.user.logRedux}
            onChange={onLogReduxChanged}
          />
        </p>
      </header>
    </div>
  );
};

export default App;
