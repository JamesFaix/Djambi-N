import { Session, Event, Game, GamesQuery } from '../api/model';

export interface GameState {
    game : Game,
    history : Event[]
}

export interface GamesQueryState {
    query : GamesQuery,
    results : Game[]
}

export interface AppState {
    session : Session,
    requests : RequestState,
    currentGame : GameState,
    gamesQuery : GamesQueryState
}

export interface RequestState {
    loginPending : boolean,
    logoutPending : boolean,
    signupPending : boolean,
    loadGamePending : boolean,
    gamesQueryPending : boolean
}

export function defaultState() : AppState {
    return {
        session: null,
        requests:{
            loginPending:false,
            logoutPending:false,
            signupPending:false,
            loadGamePending:false,
            gamesQueryPending:false
        },
        currentGame: null,
        gamesQuery: null
    };
}