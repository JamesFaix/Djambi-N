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
import { User, Game, Event, StateAndEventResponse } from '../api/model';
import '../index.css';
import Debug from '../debug';
import SnapshotsPage from './pages/snapshotsPage';
import Environment from '../environment';
import BoardViewService from '../boardRendering/boardViewService';
import { BoardView } from '../boardRendering/model';

export interface AppProps {

}

export interface AppState {
    user : User,
    eventSource : EventSource,
    game : Game,
    history : Event[],
    boardView : BoardView
}

export default class App extends React.Component<AppProps, AppState> {
    private readonly boardViewService : BoardViewService;

    constructor(props : AppProps) {
        super(props);

        K.initialize();
        Debug.Initialize();

        this.state = {
            user: null,
            eventSource: null,
            game: null,
            history: [],
            boardView: null
        };

        this.boardViewService = new BoardViewService(K.boards);
    }

    private setUser (user : User) : void {
        if (user !== null) {
            const eventSource = new EventSource(
                Environment.apiAddress() + "/notifications",
                { withCredentials: true }
            );

            eventSource.onopen = (e => {
                console.log("SSE Open");
            });

            eventSource.onmessage = (e => {
                const updateJson = e.data as string;
                const update = JSON.parse(updateJson) as StateAndEventResponse;
                if (this.state.game === null
                    || update.game.id === this.state.game.id) {
                    this.updateGame(update);
                } else {
                    //TODO: (#266) Show non-disruptive toast notification when another game you are in has been updated
                }
            });

            eventSource.onerror = (e => {
                console.log("SSE Error");
                console.log(e);
            });

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

    private async updateGame(response : StateAndEventResponse) : Promise<void> {
        const boardView = await this.boardViewService.getBoardView(response.game);

        const newHistory = [response.event];
        newHistory.push(...this.state.history);

        this.setState({
            game: response.game,
            history: newHistory,
            boardView: boardView
        });
    }

    private async loadGame(game : Game, history : Event[]) : Promise<void> {
        const boardView = await this.boardViewService.getBoardView(game);

        this.setState({
            game: game,
            history: history,
            boardView: boardView
        });
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
                                game={this.state.game}
                                load={game => this.loadGame(game, [])}
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
                                game={this.state.game}
                                history={this.state.history}
                                update={response => this.updateGame(response)}
                                load={(game, history) => this.loadGame(game, history)}
                                boardView={this.state.boardView}
                            />
                        }
                    />
                </Switch>
            </main>
        );
    }
}