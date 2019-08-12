import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GamesQuery, GameParameters, CreatePlayerRequest, User, GameStatus, Game, EventsQuery, ResultsDirection } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";
import { navigateTo } from './history';

//#region Session actions

export function login(request : LoginRequest) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.loginRequest(request));
        return Api.login(request)
            .then(session => {
                dispatch(Actions.loginSuccess(session.user));
                navigateTo(Routes.dashboard);
                return queryGamesForUser(session.user, dispatch);
            })
            .catch(_ => {
                dispatch(Actions.loginError());
            });
    };
}

export function logout() {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.logoutRequest());
        return Api.logout()
            .then(_ => {
                dispatch(Actions.logoutSuccess());
                navigateTo(Routes.login);
            })
            .catch(_ => {
                dispatch(Actions.logoutError());
            });
    };
}

export function signup(request : CreateUserRequest) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.signupRequest(request));
        return Api.createUser(request)
            .then(user => {
                dispatch(Actions.signupSuccess(user));
                const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
                return login(loginRequest)(dispatch);
            })
            .catch(_ => {
                dispatch(Actions.signupError());
            });
    };
}

export function restoreSession() {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.restoreSessionRequest());
        return Api.getCurrentUser()
            .then(user => {
                dispatch(Actions.restoreSessionSuccess(user));
                return queryGamesForUser(user, dispatch);
            })
            .catch(error => {
                let [status, message] = error;
                if (status !== 401) {
                    dispatch(Actions.restoreSessionError());
                }
            });
    };
}

export function redirectToDashboardIfLoggedIn() {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.restoreSessionRequest());
        return Api.getCurrentUser()
            .then(user => {
                dispatch(Actions.restoreSessionSuccess(user));
                navigateTo(Routes.dashboard);
                return queryGamesForUser(user, dispatch);
            })
            .catch(error => {
                let [status, message] = error;
                if (status !== 401) {
                    dispatch(Actions.restoreSessionError());
                }
            });
    };
}

export function redirectToLoginIfNotLoggedIn() {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.restoreSessionRequest());
        return Api.getCurrentUser()
            .then(user => {
                dispatch(Actions.restoreSessionSuccess(user))
                return queryGamesForUser(user, dispatch);
            })
            .catch(error => {
                let [status, message] = error;
                if (status !== 401) {
                    dispatch(Actions.restoreSessionError());
                }
                navigateTo(Routes.login);
            });
    };
}

export function redirectToLoginOrDashboard() {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.restoreSessionRequest());
        return Api.getCurrentUser()
            .then(user => {
                dispatch(Actions.restoreSessionSuccess(user));
                navigateTo(Routes.dashboard);
                return queryGamesForUser(user, dispatch);
            })
            .catch(error => {
                let [status, message] = error;
                if (status === 401) {
                    navigateTo(Routes.login);
                }
                else {
                    dispatch(Actions.restoreSessionError());
                }
            });
    };
}

//#endregion

//#region Game actions

export function loadGame(gameId: number) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.loadGameRequest(gameId));
        return Api.getGame(gameId)
            .then(game => {
                dispatch(Actions.loadGameSuccess(game));
            })
            .catch(_ => {
                dispatch(Actions.loadGameError());
            });
    };
}

export function loadGameHistory(gameId: number) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.loadGameHistoryRequest(gameId));
        const query : EventsQuery = {
            maxResults : null,
            direction : ResultsDirection.Descending,
            thresholdTime : null,
            thresholdEventId : null
        };

        return Api.getEvents(gameId, query)
            .then(history => {
                dispatch(Actions.loadGameHistorySuccess(history));
            })
            .catch(_ => {
                dispatch(Actions.loadGameHistoryError());
            });
    };
}

export function createGame(formData : GameParameters) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.createGameRequest(formData));
        return Api.createGame(formData)
            .then(game => {
                dispatch(Actions.createGameSuccess(game));
                navigateTo(Routes.lobby(game.id));
            })
            .catch(_ => {
                dispatch(Actions.createGameError())
            });
    }
}

export function addPlayer(gameId: number, request : CreatePlayerRequest) {
    return function (dispatch: Dispatch) : Promise<void> {
        dispatch(Actions.addPlayerRequest(request));
        return Api.addPlayer(gameId, request)
            .then(resp => {
                dispatch(Actions.addPlayerSuccess(resp.game));
            })
            .catch(_ => {
                dispatch(Actions.addPlayerError());
            });
    }
}

export function removePlayer(gameId: number, playerId: number) {
    return function (dispatch: Dispatch) : Promise<void> {
        dispatch(Actions.removePlayerRequest(playerId));
        return Api.removePlayer(gameId, playerId)
            .then(resp => {
                dispatch(Actions.removePlayerSuccess(resp.game));
            })
            .catch(_ => {
                dispatch(Actions.removePlayerError());
            });
    }
}

export function startGame(gameId: number) {
    return function (dispatch: Dispatch) : Promise<void> {
        dispatch(Actions.startGameRequest(gameId));
        return Api.startGame(gameId)
            .then(resp => {
                dispatch(Actions.startGameSuccess(resp.game));
                navigateTo(Routes.play(gameId));
            })
            .catch(_ => {
                dispatch(Actions.startGameError());
            });
    }
}

//#endregion

//#region Games search

export function queryGames(query: GamesQuery) {
    return function (dispatch : Dispatch) : Promise<void> {
        dispatch(Actions.queryGamesRequest(query));
        return Api.getGames(query)
            .then(games => {
                dispatch(Actions.queryGamesSuccess(games));
            })
            .catch(_ => {
                dispatch(Actions.queryGamesError());
            });
    };
}

//#endregion

//#region Helper functions

function queryGamesForUser(user: User, dispatch: Dispatch) : Promise<void> {
    const query = ModelFactory.emptyGamesQuery();
    query.playerUserName = user.name;
    dispatch(Actions.updateGamesQuery(query));
    return queryGames(query)(dispatch)
}

export function navigateToGame(game : Game) : void {
    const route = game.status === GameStatus.InProgress
        ? Routes.play(game.id)
        : Routes.lobby(game.id);
    navigateTo(route);
}

//#endregion