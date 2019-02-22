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
import "../index.css";
import { Kernel as K } from '../kernel';

export interface AppProps {

}

export interface AppState {
    user : User
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);

        K.initialize();

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
                        exact path={K.routes.home()}
                        render={_ =>
                            <HomePage
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={K.routes.signup()}
                        render={_ =>
                            <SignupPage
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={K.routes.login()}
                        render={_ =>
                            <LoginPage
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={K.routes.dashboard()}
                        render={_ =>
                            <DashboardPage
                                user={this.state.user}
                                setUser={user => this.setState({user : user})}
                            />
                        }
                    />
                    <Route
                        path={K.routes.myGames()}
                        render={_ =>
                            <MyGamesPage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={K.routes.createGame()}
                        render={_ =>
                            <CreateGamePage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={K.routes.findGame()}
                        render={_ =>
                            <FindGamePage
                                user={this.state.user}
                            />
                        }
                    />
                    <Route
                        path={K.routes.gameInfoTemplate()}
                        render={props =>
                            <GameInfoPage
                                user={this.state.user}
                                gameId={props.match.params.gameId}
                            />
                        }
                    />
                    <Route
                        path={K.routes.gameTemplate()}
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