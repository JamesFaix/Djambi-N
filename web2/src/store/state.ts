import { Event, Game, GamesQuery, User, GameParameters } from '../api/model';
import * as ModelFactory from '../api/modelFactory';

export interface AppState {
    session: SessionState,
    activeGame : ActiveGameState,
    gamesQuery : GamesQueryState,
    createGameForm : CreateGameFormState,
    navigation : NavigationState
}

export interface SessionState {
    user : User,
    signupPending : boolean,
    loginPending : boolean,
    logoutPending : boolean,
    restoreSessionPending : boolean
}

export interface ActiveGameState {
    loadGamePending : boolean,
    addPlayerPending : boolean,
    removePlayerPending : boolean,
    startGamePending : boolean,
    game : Game,
    history : Event[]
}

export interface GamesQueryState {
    query : GamesQuery,
    results : Game[],
    queryPending : boolean
}

export interface CreateGameFormState {
    parameters : GameParameters,
    createGamePending : boolean
}

export interface NavigationState {
    enableLogin ?: boolean,
    enableSignup ?: boolean,
    enableDashboard ?: boolean,
    enableCreateGame ?: boolean,
    enableLobby ?: boolean,
    enablePlay ?: boolean,
    gameId ?: number
}

export class StateFactory {
    static defaultSesssionState() : SessionState {
        return {
            user: null,
            loginPending: false,
            logoutPending: false,
            signupPending: false,
            restoreSessionPending: false
        };
    }

    static defaultActiveGameState() : ActiveGameState {
        return {
            game: null,
            history: null,
            loadGamePending: false,
            addPlayerPending: false,
            removePlayerPending: false,
            startGamePending: false
        };
    }

    static defaultGamesQueryState() : GamesQueryState {
        return {
            query: ModelFactory.emptyGamesQuery(),
            results: [],
            queryPending: false
        };
    }

    static defaultCreateGameFormState() : CreateGameFormState {
        return {
            parameters: ModelFactory.defaultGameParameters(),
            createGamePending: false
        };
    }

    static defaultNavigationState() : NavigationState {
        return {};
    }

    static defaultAppState() : AppState {
        return {
            session: StateFactory.defaultSesssionState(),
            activeGame: StateFactory.defaultActiveGameState(),
            gamesQuery: StateFactory.defaultGamesQueryState(),
            createGameForm: StateFactory.defaultCreateGameFormState(),
            navigation: StateFactory.defaultNavigationState()
        };
    }
}