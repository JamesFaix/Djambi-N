import { Session } from '../api/model';

export interface AppState {
    session : Session,
    requests : RequestState
}

export interface RequestState {
    loginPending : boolean,
    logoutPending : boolean,
    signupPending : boolean
}

export function defaultState() : AppState {
    return {
        session: null,
        requests:{
            loginPending:false,
            logoutPending:false,
            signupPending:false
        }
    };
}