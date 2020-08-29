import React, {
  FC, useEffect,
} from 'react';
import {
  BrowserRouter, Switch, Route,
} from 'react-router-dom';
import { ThemeProvider, makeStyles } from '@material-ui/core';
import NavigationDrawer from '../NavigationDrawer/NavigationDrawer';
import { loadConfig } from '../../controllers/configController';
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
import GameInfoPage from '../pages/GameInfoPage';
import NotificationsPage from '../pages/NotificationsPage';
import LatestNotificationSnackbar from '../notifications/LatestNotificationSnackBar';
import { loadGame, blockGameLoading } from '../../controllers/gameController';

const useStyles = makeStyles({
  page: {
    textAlign: 'center',
    padding: '20px',
    background: '#161616',
  },
});

const getGameId = (): number | null => {
  const url = window.location.href;
  const regex = new RegExp('.*games/(\\d+).*');
  const match = regex.exec(url);
  const gameId = match?.[1];
  return gameId ? Number(gameId) : null;
};

const App: FC = () => {
  useEffect(() => {
    // All API calls must happen after config is loaded, because that sets the API URL.

    const gameId = getGameId();

    if (gameId) {
      // If the URL has a gameID when the app first loads, load that game.
      // Game pages will also load the game because of in-app navigation.
      // First block the pages from redundantly loading the game.
      blockGameLoading();
      loadConfig()
        .then(() => restoreSession())
        .then(() => loadGame(gameId, true));
    } else {
      loadConfig()
        .then(() => restoreSession());
    }
  }, []);

  const classes = useStyles();

  return (
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <RedirectBasedOnStore />
        <NavigationDrawer />
        <LatestNotificationSnackbar />
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
            <Route path={Routes.notifications} component={NotificationsPage} />
            <Route path={Routes.newGame} component={CreateGamePage} />
            <Route path={Routes.searchGames} component={SearchGamesPage} />
            {/* Active game pages */}
            <Route
              path={Routes.gameDiplomacyTemplate}
              render={(props) => <GameDiplomacyPage gameId={props.match.params.gameId} />}
            />
            <Route
              path={Routes.gameInfoTemplate}
              render={(props) => <GameInfoPage gameId={props.match.params.gameId} />}
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
  );
};

export default App;
