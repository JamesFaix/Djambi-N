import React, { FC, useEffect, ChangeEvent } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig } from '../../utilities/configService';
import { RootState } from '../../redux/root';
import { userConfigChanged } from '../../redux/config/actionFactory';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  const selectConfig = (state: RootState) => state.config;
  const config = useSelector(selectConfig);
  const dispatch = useDispatch();

  const onLogReduxChanged = (e: ChangeEvent<HTMLInputElement>) => {
    const isChecked = e.target.checked;
    const action = userConfigChanged({
      ...config.user,
      logRedux: isChecked,
    });
    dispatch(action);
  };

  const onFavoriteWordChanged = (e: ChangeEvent<HTMLInputElement>) => {
    const word = e.target.value;
    const action = userConfigChanged({
      ...config.user,
      favoriteWord: word,
    });
    dispatch(action);
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
