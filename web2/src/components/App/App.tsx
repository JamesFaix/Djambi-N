import React, {
  FC, useEffect, ChangeEvent, useState,
} from 'react';
import { useSelector, useDispatch } from 'react-redux';
import logo from '../../assets/logo.svg';
import './App.css';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig, setUserConfig } from '../../utilities/configService';
import {
  LoginRequestDto, ApiSessionsPostRequest, CreateUserRequestDto, ApiUsersPostRequest,
} from '../../api-client';
import { loggedIn } from '../../redux/session/actionFactory';
import { apiService } from '../../utilities/apiService';
import { selectConfig } from '../../hooks/selectors';
import SignupForm from '../pages/SignupForm/SignupForm';

const App: FC = () => {
  useEffect(() => {
    loadConfig();
  }, []);

  const config = useSelector(selectConfig);
  const dispatch = useDispatch();

  const [loginRequest, setLoginRequest] = useState<LoginRequestDto>({
    username: '',
    password: '',
  });

  const [createUserRequest, setCreateUserRequest] = useState<CreateUserRequestDto>({
    name: '',
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

  // #region Login

  const onLoginUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setLoginRequest({
      ...loginRequest,
      username: e.target.value,
    });
  };

  const onLoginPasswordChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setLoginRequest({
      ...loginRequest,
      password: e.target.value,
    });
  };

  const onLoginClicked = () => {
    const request: ApiSessionsPostRequest = {
      loginRequestDto: loginRequest,
    };

    apiService.sessions.apiSessionsPost(request)
      .then((session) => {
        const action = loggedIn(session.user);
        dispatch(action);
      });

    return false;
  };

  // #endregion

  // #region Signup

  const onSignupUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setCreateUserRequest({
      ...createUserRequest,
      name: e.target.value,
    });
  };

  const onSignupPasswordChanged = (e: ChangeEvent<HTMLInputElement>) => {
    setCreateUserRequest({
      ...createUserRequest,
      password: e.target.value,
    });
  };

  const onSignupClicked = () => {
    const request: ApiUsersPostRequest = {
      createUserRequestDto: createUserRequest,
    };

    apiService.users.apiUsersPost(request);

    return false;
  };

  // #endregion

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
        <form>
          <h2>Login</h2>
          Username:
          <input
            type="text"
            value={loginRequest.username}
            onChange={onLoginUsernameChanged}
          />
          <br />
          Password:
          <input
            type="password"
            value={loginRequest.password}
            onChange={onLoginPasswordChanged}
          />
          <br />
          <button
            type="button"
            onClick={onLoginClicked}
          >
            Login
          </button>
        </form>
        <SignupForm />

      </header>
    </div>
  );
};

export default App;
