import { Event, Game, GamesQuery, User, GameParameters, Board, PieceKind } from '../api/model';
import * as ModelFactory from '../api/modelFactory';
import { BoardView, Point } from '../viewModel/board/model';

export interface AppState {
    session: SessionState,
    activeGame : ActiveGameState,
    gamesQuery : GamesQueryState,
    createGameForm : CreateGameFormState,
    navigation : NavigationState,
    boards : BoardsState,
    display : DisplayState,
    images : ImagesState
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
    loadHistoryPending : boolean,
    addPlayerPending : boolean,
    removePlayerPending : boolean,
    startGamePending : boolean,
    selectionPending : boolean,
    game : Game,
    history : Event[],
    boardView : BoardView,
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

export interface BoardsState {
    loadBoardPending : boolean,
    boards : Map<number, Board>
}

export interface DisplayState {
    boardZoomLevel : number,
    boardScrollPercent : Point,
    boardContainerSize : Point,
    canvasMargin : number,
    canvasContentPadding : number,
}

export interface ImagesState {
    pieces : Map<PieceKind, HTMLImageElement>
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
            loadHistoryPending: false,
            addPlayerPending: false,
            removePlayerPending: false,
            startGamePending: false,
            selectionPending: false,
            boardView: null
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

    static defaultBoardsState() : BoardsState {
        return {
            loadBoardPending: false,
            boards: new Map<number, Board>()
        };
    }

    static defaultDisplayState() : DisplayState {
        return {
            boardZoomLevel: 0,
            boardScrollPercent: { x: 0.5, y: 0.5 },
            boardContainerSize: { x: 1000, y: 1000 },
            canvasContentPadding: 5,
            canvasMargin: 5,
        };
    }

    static defaultImagesState() : ImagesState {
        return {
            pieces: new Map<PieceKind, HTMLImageElement>()
        };
    }

    static defaultAppState() : AppState {
        return {
            session: StateFactory.defaultSesssionState(),
            activeGame: StateFactory.defaultActiveGameState(),
            gamesQuery: StateFactory.defaultGamesQueryState(),
            createGameForm: StateFactory.defaultCreateGameFormState(),
            navigation: StateFactory.defaultNavigationState(),
            boards: StateFactory.defaultBoardsState(),
            display: StateFactory.defaultDisplayState(),
            images : StateFactory.defaultImagesState()
        };
    }
}