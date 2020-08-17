import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
import { ThemeProvider, makeStyles } from '@material-ui/core';
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
import TopBar from '../TopBar/TopBar';
import SignOutPage from '../pages/SignOutPage';
import { theme } from '../../styles/materialTheme';

const useStyles = makeStyles({
  app: {
    textAlign: 'center',
    height: '100%',
  },
  page: {
    padding: '20px',
    background: '#161616',
    height: '100%',
  },
});

const App: FC = () => {
  useEffect(() => {
    loadConfig()
      .then(() => restoreSession()); // Must happen after environment config is loaded
  }, []);

  const classes = useStyles();

  return (
    <div className={classes.app}>
      <ThemeProvider theme={theme}>
        <BrowserRouter>
          <RedirectBasedOnStore />
          <NavigationDrawer />
          <TopBar />
          <div className={classes.page}>
            <Switch>
              {/* Gameless pages */}
              <Route path={Routes.settings} component={UserConfigPage} />
              <Route path={Routes.signIn} component={SignInPage} />
              <Route path={Routes.signOut} component={SignOutPage} />
              <Route path={Routes.createAccount} component={CreateAccountPage} />
              <Route path={Routes.rules} component={RulesPage} />
              <Route path={Routes.home} component={HomePage} />
              <Route path={Routes.newGame} component={CreateGamePage} />
              <Route path={Routes.searchGames} component={SearchGamesPage} />
              {/* Active game pages */}
              <Route
                path={Routes.gameDiplomacyTemplate}
                render={(props) => <GameDiplomacyPage gameId={props.match.params.gameId} />}
              />
              <Route
                path={Routes.gameLobbyTemplate}
                render={(props) => <GameLobbyPage gameId={props.match.params.gameId} />}
              />
              <Route
                path={Routes.gameOutcomeTemplate}
                render={(props) => <GameOutcomePage gameId={props.match.params.gameId} />}
              />
              <Route
                path={Routes.gamePlayTemplate}
                render={(props) => <GamePlayPage gameId={props.match.params.gameId} />}
              />
              <Route
                path={Routes.gameSnapshotsTemplate}
                render={(props) => <GameSnapshotsPage gameId={props.match.params.gameId} />}
              />
              <Route
                path={Routes.gameTemplate}
                render={(props) => <GamePage gameId={props.match.params.gameId} />}
              />
              {/* Misc pages */}
              <Route component={NoMatchPage} />
            </Switch>
          </div>
        </BrowserRouter>
      </ThemeProvider>
    </div>
  );
};

export default App;
