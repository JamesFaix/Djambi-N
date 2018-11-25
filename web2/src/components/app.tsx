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
import MyGamesPage from './pages/myGamesPage';
import CreateLobbyPage from './pages/createLobbyPage';
import FindLobbyPage from './pages/findLobbyPage';
import LobbyPage from './pages/lobbyPage';

export interface AppProps {

}

export interface AppState {
    user : UserResponse,
    lobbyId : number,
    api : ApiClient
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);
        this.state = {
            user : null,
            lobbyId : null,
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
                                setLobbyId={lobbyId => this.setState({lobbyId : lobbyId})}
                                rulesUrl={this.rulesUrl}
                            />
                        }
                    />
                    <Route
                        path='/myGames'
                        render={_ =>
                            <MyGamesPage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path='/createGame'
                        render={_ =>
                            <CreateLobbyPage
                                user={this.state.user}
                                api={this.state.api}
                                lobbyId={this.state.lobbyId}
                                setLobbyId={lobbyId => this.setState({lobbyId : lobbyId})}
                            />
                        }
                    />
                    <Route
                        path='/findGame'
                        render={_ =>
                            <FindLobbyPage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path='/lobby/:lobbyId'
                        render={props =>
                            <LobbyPage
                                user={this.state.user}
                                api={this.state.api}
                                lobbyId={props.match.params.lobbyId}
                                setLobbyId={lobbyId => this.setState({lobbyId : lobbyId})}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}