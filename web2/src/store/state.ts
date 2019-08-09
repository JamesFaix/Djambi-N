import { Event, Game, GamesQuery, User } from '../api/model';

export interface GameState {
    game : Game,
    history : Event[]
}

export interface GamesQueryState {
    query : GamesQuery,
    results : Game[]
}

export interface AppState {
    user : User,
    requests : RequestState,
    currentGame : GameState,
    gamesQuery : GamesQueryState,
    redirectRoute : string
}

export interface RequestState {
    loginPending : boolean,
    logoutPending : boolean,
    signupPending : boolean,
    loadGamePending : boolean,
    gamesQueryPending : boolean,
    restoreSessionPending : boolean
}

export function defaultState() : AppState {
    return {
        user: null,
        requests:{
            loginPending:false,
            logoutPending:false,
            signupPending:false,
            loadGamePending:false,
            gamesQueryPending:false,
            restoreSessionPending:false
        },
        currentGame: null,
        gamesQuery: null,
        redirectRoute: null
    };
}