import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";

export function login(request : LoginRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.loginRequest(request));
        Api.login(request)
            .then(session => dispatch(Actions.loginSuccess(session)))
            .catch(_ => dispatch(Actions.loginError()));
    }
}

export function logout() {
    return function (dispatch : Dispatch) {
        dispatch(Actions.logoutRequest());
        Api.logout()
            .then(_ => dispatch(Actions.logoutSuccess()))
            .catch(_ => dispatch(Actions.logoutError()));
    }
}

export function signup(request : CreateUserRequest) {
    return function (dispatch : Dispatch) {
        dispatch(Actions.signupRequest(request));
        Api.createUser(request)
            .then(user => {
                dispatch(Actions.signupSuccess(user));
                const loginRequest : LoginRequest = {
                    username: request.name,
                    password: request.password
                };
                return login(loginRequest);
            })
            .catch(_ => dispatch(Actions.signupError()));
    }
}