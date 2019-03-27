import * as React from 'react';
import CreateGamePage from './pages/createGamePage';
import DashboardPage from './pages/dashboardPage';
import FindGamePage from './pages/findGamePage';
import GameInfoPage from './pages/gameInfoPage';
import GamePage from './pages/gamePage';
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
import SnapshotsPage from './pages/snapshotsPage';
import Environment from '../environment';

export interface AppProps {

}

export interface AppState {
    user : User,
    eventSource : EventSource
}

export default class App extends React.Component<AppProps, AppState> {
    constructor(props : AppProps) {
        super(props);

        K.initialize();
        Debug.Initialize();

        this.state = {
            user : null,
            eventSource : null
        };
    }

    private setUser (user : User) : void {
        console.log("set user to " + user);
        if (user !== null) {
            const eventSource = new EventSource(
                Environment.apiAddress() + "/notifications",
                { withCredentials: true }
            );

            eventSource.onmessage = (e => console.log(e));

            this.setState({
                user: user,
                eventSource: eventSource
            });
        } else {
            if (this.state.eventSource !== null) {
                this.state.eventSource.close();
            }
            this.setState({
                user: null,
                eventSource: null
            });
        }
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
                                setUser={user => this.setUser(user)}
                            />
                        }
                    />
                    <Route
                        path={K.routes.signup()}
                        render={_ =>
                            <SignupPage
                                user={this.state.user}
                                setUser={user => this.setUser(user)}
                            />
                        }
                    />
                    <Route
                        path={K.routes.login()}
                        render={_ =>
                            <LoginPage
                                user={this.state.user}
                                setUser={user => this.setUser(user)}
                            />
                        }
                    />
                    <Route
                        path={K.routes.dashboard()}
                        render={_ =>
                            <DashboardPage
                                user={this.state.user}
                                setUser={user => this.setUser(user)}
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

                    {/* Order here is really important, longer routes must go first. */}
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
                        path={K.routes.snapshotsTemplate()}
                        render={props =>
                            <SnapshotsPage
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