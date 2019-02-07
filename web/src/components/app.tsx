import * as React from 'react';
import { Switch, Route } from 'react-router-dom';
import '../index.css';

import HomePage from './pages/homePage';
import LoginPage from './pages/loginPage';
import SignupPage from './pages/signupPage';
import DashboardPage from './pages/dashboardPage';
import GamePage from './pages/gamePage/gamePage';

import TopMenu from './topMenu';
import { User } from '../api/model';
import ApiClient from '../api/client';
import MyGamesPage from './pages/myGamesPage';
import CreateGamePage from './pages/createGamePage';
import FindGamePage from './pages/findGamePage';
import GameInfoPage from './pages/gameInfoPage/gameInfoPage';
import Routes from '../routes';
import ThemeService from '../themes/themeService';

export interface AppProps {

}

export interface AppState {
    user : User,
    api : ApiClient,
    theme : ThemeService
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);
        this.state = {
            user : null,
            api : new ApiClient(),
            theme : new ThemeService()
        };
    }

    render() {
        return (
            <main>
                <TopMenu/>
                <Switch>
                    <Route
                        exact path={Routes.home()}
                        render={_ =>
                            <HomePage
                                user={this.state.user}
                                api={this.state.api}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.signup()}
                        render={_ =>
                            <SignupPage
                                api={this.state.api}
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.login()}
                        render={_ =>
                            <LoginPage
                                api={this.state.api}
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.dashboard()}
                        render={_ =>
                            <DashboardPage
                                user={this.state.user}
                                api={this.state.api}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.myGames()}
                        render={_ =>
                            <MyGamesPage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path={Routes.createGame()}
                        render={_ =>
                            <CreateGamePage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path={Routes.findGame()}
                        render={_ =>
                            <FindGamePage
                                user={this.state.user}
                                api={this.state.api}
                            />
                        }
                    />
                    <Route
                        path={Routes.gameInfoTemplate()}
                        render={props =>
                            <GameInfoPage
                                user={this.state.user}
                                api={this.state.api}
                                gameId={props.match.params.gameId}
                            />
                        }
                    />
                    <Route
                        path={Routes.gameTemplate()}
                        render={props =>
                            <GamePage
                                user={this.state.user}
                                api={this.state.api}
                                gameId={props.match.params.gameId}
                                theme={this.state.theme}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}