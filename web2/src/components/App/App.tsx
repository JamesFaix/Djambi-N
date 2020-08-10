import React, {
  FC, useEffect, ChangeEvent, useState,
} from 'react';
import { useSelector } from 'react-redux';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig, setUserConfig } from '../../utilities/configService';
import { RootState } from '../../redux/root';
import { LoginRequestDto } from '../../api-client';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  const selectConfig = (state: RootState) => state.config;
  const config = useSelector(selectConfig);

  const [loginRequest, setLoginRequest] = useState<LoginRequestDto>({
    username: '',
    password: '',
  });

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

  const onUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setLoginRequest({
      ...loginRequest,
      username: e.target.value,
    });
  };

  const onPasswordChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setLoginRequest({
      ...loginRequest,
      password: e.target.value,
    });
  };

  const onLoginClicked = () => {
    console.log('login clicked');
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
        <p>
          <form>
            <h2>Login</h2>
            Username:
            <input
              type="text"
              value={loginRequest.username}
              onChange={onUsernameChanged}
            />
            <br />
            Password:
            <input
              type="password"
              value={loginRequest.password}
              onChange={onPasswordChanged}
            />
            <br />
            <button
              type="submit"
              onClick={onLoginClicked}
            >
              Login
            </button>
          </form>
        </p>
      </header>
    </div>
  );
};

export default App;
