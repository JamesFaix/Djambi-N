import * as React from 'react';
import { Switch, Route } from 'react-router-dom';
import '../index.css';

import HomePage from './pages/homePage';
import LoginPage from './pages/loginPage';
import SignupPage from './pages/signupPage';
import DashboardPage from './pages/dashboardPage';
import GamePage from './pages/gamePage';

import TopMenu from './topMenu';
import { User } from '../api/model';
import ApiClient from '../api/client';
import MyGamesPage from './pages/myGamesPage';
import CreateGamePage from './pages/createGamePage';
import FindGamePage from './pages/findGamePage';
import GameInfoPage from './pages/gameInfoPage';

export interface AppProps {

}

export interface AppState {
    user : User,
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

    private rulesUrl = "https://github.com/GamesFaix/Djambi3/blob/master/docs/Rules.md";

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
                                rulesUrl={this.rulesUrl}
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
                                rulesUrl={this.rulesUrl}
                            />
                        }
                    />
                    <Route
                        path='/games/my'
                        render={_ =>
                            <MyGamesPage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path='/games/create'
                        render={_ =>
                            <CreateGamePage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path='/games/find'
                        render={_ =>
                            <FindGamePage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path='/games/:gameId/info'
                        render={props =>
                            <GameInfoPage
                                user={this.state.user}
                                api={this.state.api}
                                gameId={props.match.params.gameId}
                            />
                        }
                    />
                    <Route
                        path='/games/:gameId'
                        render={props =>
                            <GamePage
                                user={this.state.user}
                                api={this.state.api}
                                gameId={props.match.params.gameId}
                                rulesUrl={this.rulesUrl}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}