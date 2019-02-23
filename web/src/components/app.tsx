import * as React from 'react';
import CreateGamePage from './pages/createGamePage';
import DashboardPage from './pages/dashboardPage';
import FindGamePage from './pages/findGamePage';
import GameInfoPage from './pages/gameInfoPage/gameInfoPage';
import GamePage from './pages/gamePage/gamePage';
import HomePage from './pages/homePage';
import LoginPage from './pages/loginPage';
import MyGamesPage from './pages/myGamesPage';
import SignupPage from './pages/signupPage';
import TopMenu from './topMenu';
import { Kernel as K } from '../kernel';
import { Route, Switch } from 'react-router-dom';
import { User } from '../api/model';
import '../index.css';
import Debug from '../debug';

export interface AppProps {

}

export interface AppState {
    user : User
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);

        K.initialize();
        Debug.Initialize();

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