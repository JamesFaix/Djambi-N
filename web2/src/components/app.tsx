import * as React from 'react';
import TopBar from './topBar/topBar';
import { Route } from 'react-router';
import Routes from '../routes';
import SignupPage from './pages/signupPage';
import LoginPage from './pages/loginPage';
import DashboardPage from './pages/dashboardPage';
import { Switch } from 'react-router-dom';
import * as Redirects from './redirects';

const App : React.SFC<{}> = _ => {
    return (
        <div>
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
                    path={Routes.base}
                    render={_ => <Redirects.ToHome/>}
                />
            </Switch>
        </div>
    );
};

export default App;