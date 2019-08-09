import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GamesQuery } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";

export function login(request : LoginRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.loginRequest(request));
        Api.login(request)
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
        Api.logout()
            .then(_ => dispatch(Actions.logoutSuccess()))
            .catch(_ => dispatch(Actions.logoutError()));
    };
}

export function signup(request : CreateUserRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.signupRequest(request));
        Api.createUser(request)
            .then(user => {
                dispatch(Actions.signupSuccess(user));
                const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
                return login(loginRequest);
            })
            .catch(_ => dispatch(Actions.signupError()));
    };
}

export function loadGame(gameId: number) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.loadGameRequest(gameId));
        return Api.getGame(gameId)
            .then(game => {
                dispatch(Actions.loadGameSuccess(game));
                dispatch(Actions.redirectPending(Routes.lobby(gameId)));
            })
            .catch(_ => dispatch(Actions.loadGameError()));
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
        Api.getCurrentUser()
            .then(user => dispatch(Actions.restoreSessionSuccess(user)))
            .catch(error => {
                let [status, message] = error;
                if (status !== 401) {
                    dispatch(Actions.restoreSessionError());
                }
            });
    };
}
