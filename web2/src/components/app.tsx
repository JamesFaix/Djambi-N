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

const App : React.SFC<{}> = _ => {
    return (
        <div>
            <SessionRestorer/>
            <TopBar/>
            <Switch>
                <Route
                    path={Routes.signup}
                    render={_ => <SignupPage/>}
                />
                <Route
                    path={Routes.login}
                    render={_ => <LoginPage/>}
                />
                <Route
                    path={Routes.dashboard}
                    render={_ => <DashboardPage/>}
                />
                <Route
                    path={Routes.lobbyPattern}
                    render={_ => <LobbyPage/>}
                />
                <Route
                    path={Routes.base}
                    render={_ => <Redirects.ToHome/>}
                />
            </Switch>
        </div>
    );
};

export default App;