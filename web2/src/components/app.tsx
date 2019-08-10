import * as React from 'react';
import TopBar from './topBar/topBar';
import { Route } from 'react-router';
import Routes from '../routes';
import SignupPage from './pages/signupPage';
import LoginPage from './pages/loginPage';
import DashboardPage from './pages/dashboardPage';
import { Switch } from 'react-router-dom';
import * as Redirects from './redirects';
import LobbyPage from './pages/lobbyPage';
import SessionRestorer from './sessionRestorer';
import CreateGamePage from './pages/createGamePage';
import PlayPage from './pages/playPage';

const App : React.SFC<{}> = _ => {
    return (
        <div>
            <SessionRestorer/>
            <TopBar/>
            <Switch>
                <Route
                    path={Routes.signup}
                    component={SignupPage}
                />
                <Route
                    path={Routes.login}
                    component={LoginPage}
                />
                <Route
                    path={Routes.dashboard}
                    component={DashboardPage}
                />
                <Route
                    path={Routes.createGame}
                    component={CreateGamePage}
                />
                <Route
                    path={Routes.lobbyPattern}
                    component={LobbyPage}
                />
                <Route
                    path={Routes.playPattern}
                    component={PlayPage}
                />
                <Route
                    path={Routes.base}
                    component={Redirects.ToHome}
                />
            </Switch>
        </div>
    );
};

export default App;