import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GamesQuery, GameParameters } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";
import { navigateTo } from './history';

export function login(request : LoginRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.loginRequest(request));
        return Api.login(request)
            .then(session => {
                dispatch(Actions.loginSuccess(session.user))

                const query = ModelFactory.emptyGamesQuery();
                query.playerUserName = session.user.name;
                dispatch(Actions.updateGamesQuery(query));
                queryGames(query)(dispatch)
            })
            .catch(_ => dispatch(Actions.loginError()));
    };
}

export function logout() {
    return function (dispatch : Dispatch) {
        dispatch(Actions.logoutRequest());
        return Api.logout()
            .then(_ => dispatch(Actions.logoutSuccess()))
            .catch(_ => dispatch(Actions.logoutError()));
    };
}

export function signup(request : CreateUserRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.signupRequest(request));
        return Api.createUser(request)
            .then(user => {
                dispatch(Actions.signupSuccess(user));
                const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
                return login(loginRequest);
            })
            .catch(_ => dispatch(Actions.signupError()));
    };
}

export function loadGame(gameId: number) {
    return async function (dispatch : Dispatch) {
        dispatch(Actions.loadGameRequest(gameId));
        try {
            const game = await Api.getGame(gameId);
            dispatch(Actions.loadGameSuccess(game));
            navigateTo(Routes.lobby(gameId));
        }
        catch {
            dispatch(Actions.loadGameError());
        }
    };
}

export function queryGames(query: GamesQuery) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.queryGamesRequest(query));
        return Api.getGames(query)
            .then(games => dispatch(Actions.queryGamesSuccess(games)))
            .catch(_ => dispatch(Actions.queryGamesError()));
    };
}

export function restoreSession() {
    return function (dispatch : Dispatch) {
        dispatch(Actions.restoreSessionRequest());
        return Api.getCurrentUser()
            .then(user => {
                dispatch(Actions.restoreSessionSuccess(user))

                const query = ModelFactory.emptyGamesQuery();
                query.playerUserName = user.name;
                dispatch(Actions.updateGamesQuery(query));
                queryGames(query)(dispatch)
            })
            .catch(error => {
                let [status, message] = error;
                if (status !== 401) {
                    dispatch(Actions.restoreSessionError());
                }
            });
    };
}

export function createGame(formData : GameParameters) {
    return function (dispatch : Dispatch) {
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
