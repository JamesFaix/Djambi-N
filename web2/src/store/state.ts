import { Event, Game, Session } from '../api/model';

export interface AppState {
    currentGame : GameState,
    session : Session,
    requests : RequestState
}

export interface GameState {
    game : Game,
    history : Event[]
}

export interface RequestState {
    loginPending : boolean,
    logoutPending : boolean,
    signupPending : boolean,
    loadGamePending : boolean,
    loadGameHistoryPending : boolean
}

export function defaultState() : AppState {
    return {
        currentGame: {
            game: null,
            history: null
        },
        session: null,
        requests:{
            loginPending:false,
            logoutPending:false,
            signupPending:false,
            loadGamePending:false,
            loadGameHistoryPending:false
        }
    };
}