import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
import logo from '../../assets/logo.svg';
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
import RulesPage from '../pages/RulesPage';
import GameDiplomacyPage from '../pages/GameDiplomacyPage';
import GameLobbyPage from '../pages/GameLobbyPage';
import GameOutcomePage from '../pages/GameOutcomePage';
import GameSnapshotsPage from '../pages/GameSnapshotsPage';
import GamePage from '../pages/GamePage';
import GamePlayPage from '../pages/GamePlayPage';
import CreateGamePage from '../pages/CreateGamePage';
import HomePage from '../pages/HomePage';
import SearchGamesPage from '../pages/SearchGamesPage';

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
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" style={{ width: '200px' }} />
        </header>
        <div style={{ padding: '20px' }}>
          <Switch>
            {/* Gameless pages */}
            <Route path={Routes.settings} component={UserConfigPage} />
            <Route path={Routes.signIn} component={SignInPage} />
            <Route path={Routes.createAccount} component={CreateAccountPage} />
            <Route path={Routes.rules} component={RulesPage} />
            <Route path={Routes.home} component={HomePage} />
            <Route path={Routes.newGame} component={CreateGamePage} />
            <Route path={Routes.searchGames} component={SearchGamesPage} />
            {/* Active game pages */}
            <Route path={Routes.gameDiplomacyTemplate} component={GameDiplomacyPage} />
            <Route path={Routes.gameLobbyTemplate} component={GameLobbyPage} />
            <Route path={Routes.gameOutcomeTemplate} component={GameOutcomePage} />
            <Route path={Routes.gamePlayTemplate} component={GamePlayPage} />
            <Route path={Routes.gameSnapshotsTemplate} component={GameSnapshotsPage} />
            <Route path={Routes.gameTemplate} component={GamePage} />
            {/* Misc pages */}
            <Route component={NoMatchPage} />
          </Switch>
        </div>
      </BrowserRouter>
    </div>
  );
};

export default App;
