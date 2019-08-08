import * as React from 'react';
import { AppState } from "../store/state";
import TopBar from './topBar/topBar';
import { Repository } from '../repository';
import { Route } from 'react-router';
import Routes from '../routes';
import SignupPage from './pages/signupPage';
import LoginPage from './pages/loginPage';
import DashboardPage from './pages/dashboardPage';
import { Switch } from 'react-router-dom';
import { CustomAction } from '../store/actions';
import { Store } from 'redux';
import * as Redirects from './redirects';

interface AppProps {
    repo : Repository,
    store: Store<AppState, CustomAction>
}

const App : React.SFC<AppProps> = props => {
    return (
        <div>
            <TopBar/>
            <Switch>
                <Route
                    path={Routes.signup}
                    render={_ =>
                        <SignupPage
                            onSignupClicked={request => props.repo.signup(request)}
                        />
                    }
                />
                <Route
                    path={Routes.login}
                    render={_ =>
                        <LoginPage
                            onLoginClicked={request => props.repo.login(request)}
                        />
                    }
                />
                <Route
                    path={Routes.dashboard}
                    render={_ =>
                        <DashboardPage
                            onLogoutClicked={() => props.repo.logout()}
                        />
                    }
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