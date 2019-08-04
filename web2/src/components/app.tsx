import * as React from 'react';
import { AppState } from "../store/state";
import TopBar from './topBar/topBar';
import { Repository } from '../repository';
import { Route, Redirect } from 'react-router';
import Routes from '../routes';
import SignupPage from './pages/signupPage';
import LoginPage from './pages/loginPage';
import DashboardPage from './pages/dashboardPage';
import { Switch } from 'react-router-dom';

interface AppProps {
    appState : AppState,
    repo : Repository
}

export default class App extends React.Component<AppProps>{
    render() {
        return (
            <div>
                <TopBar appState={this.props.appState}></TopBar>
                <Switch>
                    <Route
                        path={Routes.signup}
                        render={_ =>
                            <SignupPage
                                appState={this.props.appState}
                                repo={this.props.repo}
                            />
                        }
                    />
                    <Route
                        path={Routes.login}
                        render={_ =>
                            <LoginPage
                                appState={this.props.appState}
                                repo={this.props.repo}
                            />
                        }
                    />
                    <Route
                        path={Routes.dashboard}
                        render={_ =>
                            <DashboardPage
                                appState={this.props.appState}
                                repo={this.props.repo}
                            />
                        }
                    />
                    <Route
                        path={Routes.base}
                        render={_ => {
                            const url = this.props.appState.session
                                ? Routes.dashboard
                                : Routes.login;
                            return <Redirect to={url}/>;
                        }}
                    />
                </Switch>
            </div>
        );
    }
}