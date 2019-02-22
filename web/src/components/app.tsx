import * as React from 'react';
import { Switch, Route } from 'react-router-dom';
import HomePage from './pages/homePage';
import LoginPage from './pages/loginPage';
import SignupPage from './pages/signupPage';
import DashboardPage from './pages/dashboardPage';
import GamePage from './pages/gamePage/gamePage';
import TopMenu from './topMenu';
import { User } from '../api/model';
import MyGamesPage from './pages/myGamesPage';
import CreateGamePage from './pages/createGamePage';
import FindGamePage from './pages/findGamePage';
import GameInfoPage from './pages/gameInfoPage/gameInfoPage';
import Routes from '../routes';
import "../index.css";
import Kernel from '../kernel';

export interface AppProps {

}

export interface AppState {
    user : User
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);

        Kernel.Initialize();

        this.state = {
            user : null
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
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.signup()}
                        render={_ =>
                            <SignupPage
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.login()}
                        render={_ =>
                            <LoginPage
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
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={Routes.myGames()}
                        render={_ =>
                            <MyGamesPage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={Routes.createGame()}
                        render={_ =>
                            <CreateGamePage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={Routes.findGame()}
                        render={_ =>
                            <FindGamePage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={Routes.gameInfoTemplate()}
                        render={props =>
                            <GameInfoPage
                                user={this.state.user}
                                gameId={props.match.params.gameId}
                            />
                        }
                    />
                    <Route
                        path={Routes.gameTemplate()}
                        render={props =>
                            <GamePage
                                user={this.state.user}
                                gameId={props.match.params.gameId}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}