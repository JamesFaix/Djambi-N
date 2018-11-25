import * as React from 'react';
import { Switch, Route } from 'react-router-dom';
import '../index.css';

import HomePage from './pages/homePage';
import LoginPage from './pages/loginPage';
import SignupPage from './pages/signupPage';
import DashboardPage from './pages/dashboardPage';

import TopMenu from './topMenu';
import { UserResponse } from '../api/model';
import ApiClient from '../api/client';

export interface AppProps {

}

export interface AppState {
    user : UserResponse,
    api : ApiClient
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);
        this.state = {
            user : null,
            api : new ApiClient()
        };
    }

    render() {
        return (
            <main>
                <TopMenu/>
                <Switch>
                    <Route
                        exact path='/'
                        render={_ =>
                            <HomePage
                                user={this.state.user}
                                api={this.state.api}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path='/signup'
                        render={_ =>
                            <SignupPage
                                api={this.state.api}
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path='/login'
                        render={_ =>
                            <LoginPage
                                api={this.state.api}
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path='/dashboard'
                        render={_ =>
                            <DashboardPage
                                user={this.state.user}
                                api={this.state.api}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}