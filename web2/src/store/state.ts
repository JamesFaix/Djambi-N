import { Event, Game, GamesQuery, User, GameParameters } from '../api/model';
import * as ModelFactory from '../api/modelFactory';

export interface GameState {
    game : Game,
    history : Event[]
}

export interface GamesQueryState {
    query : GamesQuery,
    results : Game[]
}

export interface CreateGameFormState {
    parameters : GameParameters
}

export interface AppState {
    user : User,
    requests : RequestState,
    currentGame : GameState,
    gamesQuery : GamesQueryState,
    createGameForm : CreateGameFormState
}

export interface RequestState {
    loginPending : boolean,
    logoutPending : boolean,
    signupPending : boolean,
    loadGamePending : boolean,
    gamesQueryPending : boolean,
    restoreSessionPending : boolean,
    createGamePending : boolean
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
            restoreSessionPending:false,
            createGamePending:false
        },
        currentGame: null,
        gamesQuery: null,
        createGameForm: {
            parameters: ModelFactory.defaultGameParameters()
        }
    };
}