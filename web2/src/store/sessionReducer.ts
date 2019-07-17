import { Session } from '../api/model';

export const LOGIN = "LOGIN";
export const LOGOUT = "LOGOUT";

interface LoginAction {
    type : typeof LOGIN,
    session : Session
}

interface LogoutAction {
    type : typeof LOGOUT
}

export type SystemActionTypes =
    LoginAction |
    LogoutAction;

export function login(session : Session) {
    return {
        type: LOGIN,
        session: session
    };
}

export function logout() {
    return {
        type: LOGOUT
    };
}

export function sessionReducer(
    state: Session,
    action: SystemActionTypes) : Session {

    switch (action.type) {
        case LOGIN:
            return action.session;
        case LOGOUT:
            return null;
        default:
            return state;
    }
}