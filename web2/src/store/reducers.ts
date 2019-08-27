import { CustomAction, DataAction, ActionTypes } from './actions';
import { Game, GamesQuery, User, GameParameters, Event, Board, PieceKind, StateAndEventResponse } from '../api/model';
import { AppState, StateFactory, NavigationState } from './state';
import BoardViewFactory from '../viewModel/board/boardViewFactory';
import CanvasTransformService, { CanvasTranformData } from '../viewModel/board/canvasTransformService';
import Geometry from '../viewModel/board/geometry';
import { Point } from '../viewModel/board/model';
import { ApiRequest, ApiResponse, ApiError } from '../api/requestModel';

export function reducer(state: AppState, action : CustomAction) : AppState {
    switch (action.type) {
        //Session actions
        case ActionTypes.Login:
            return loginReducer(state, action);
        case ActionTypes.Logout:
            return logoutReducer(state, action);
        case ActionTypes.Signup:
            return signupReducer(state, action);
        case ActionTypes.RestoreSession:
                return restoreSessionReducer(state, action);

        //Game search actions
        case ActionTypes.QueryGames:
            return queryGamesReducer(state, action);
        case ActionTypes.UpdateGamesQuery:
            return updateGamesQueryReducer(state, action);

        //Game creation actions
        case ActionTypes.CreateGame:
            return createGameReducer(state, action);
        case ActionTypes.UpdateCreateGameForm:
            return updateCreateGameFormReducer(state, action);

        //Game actions
        case ActionTypes.LoadGame:
            return loadGameReducer(state, action);
        case ActionTypes.LoadGameHistory:
            return loadGameHistoryReducer(state, action);
        case ActionTypes.LoadBoard:
            return loadBoardReducer(state, action);
        case ActionTypes.UpdateGame:
            return updateGameReducer(state, action);

            //Misc
        case ActionTypes.SetNavigationOptions:
            return setNavigationOptionsReducer(state, action);
        case ActionTypes.BoardZoom:
            return boardZoomReducer(state, action);
        case ActionTypes.BoardScroll:
            return boardScrollReducer(state, action);
        case ActionTypes.LoadPieceImage:
            return loadPieceImageReducer(state, action);

        //API
        case ActionTypes.ApiRequest:
            return apiRequestReducer(state, action);
        case ActionTypes.ApiResponse:
            return apiResponseReducer(state, action);
        case ActionTypes.ApiError:
            return apiErrorReducer(state, action);

        default:
            return state;
    }
}

//#region Session actions

function loginReducer(state: AppState, action : CustomAction) : AppState {
    const da = <DataAction<User>>action;
    const newState = {...state};
    newState.session = {...state.session};
    newState.session.user = da.data;
    return newState;
}

function logoutReducer(state: AppState, action : CustomAction) : AppState {
    //This is the one case where the normal combineReducers pattern doesn't make sense
    return StateFactory.defaultAppState();
}

function signupReducer(state: AppState, action : CustomAction) : AppState {
    const newState = {...state};
    newState.session = {...state.session};
    return newState;
}

function restoreSessionReducer(state: AppState, action : CustomAction) : AppState {
    const da = <DataAction<User>>action;
    const newState = {...state};
    newState.session = {...state.session};
    newState.session.user = da.data;
    return newState;
}

//#endregion

//#region Game search actions

function queryGamesReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<Game[]>>action;
    const newState = {...state};
    newState.gamesQuery = {
        query: state.gamesQuery.query,
        results: da.data
    };
    return newState;
}

function updateGamesQueryReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<GamesQuery>>action;
    const newState = {...state};
    newState.gamesQuery = {...state.gamesQuery};
    newState.gamesQuery.query = da.data;
    return newState;
}
//#endregion

//#region Game creation actions

function updateCreateGameFormReducer(state: AppState, action : CustomAction) : AppState {
    const da = <DataAction<GameParameters>>action;
    const newState = {...state};
    newState.createGameForm = {...state.createGameForm};
    newState.createGameForm.parameters = da.data;
    return newState;
}

function createGameReducer(state: AppState, action : CustomAction) : AppState {
    const da = <DataAction<Game>>action;
    const newState = {...state};
    newState.createGameForm = {...state.createGameForm};
    newState.activeGame = StateFactory.defaultActiveGameState();
    newState.activeGame.game = da.data;
    return newState;
}
//#endregion

//#region Game actions

function loadGameReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<Game>>action;
    const newState = {...state};
    newState.activeGame = StateFactory.defaultActiveGameState();
    newState.activeGame.game = da.data;
    updateBoardView(newState, da.data);
    return newState;
}

function loadBoardReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<Board>>action;
    const newState = {...state};
    newState.boards = {...state.boards};
    const boards = new Map<number, Board>(state.boards.boards);
    boards.set(da.data.regionCount, da.data);
    newState.boards.boards = boards;
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function loadGameHistoryReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<Event[]>>action;
    const newState = {...state};
    newState.activeGame = {...state.activeGame};
    newState.activeGame.history = da.data;
    return newState;
}

function updateGameReducer(state: AppState, action : CustomAction) : AppState {
    const da = <DataAction<StateAndEventResponse>>action;
    const game = da.data.game;
    const event = da.data.event;
    const newState = {...state};
    newState.activeGame = {...state.activeGame};
    newState.activeGame.game = game;
    newState.activeGame.history = [event].concat(state.activeGame.history);
    updateBoardView(newState, game);
    return newState;
}

function updateBoardView(state : AppState, game : Game) : void {
    const rc = game.parameters.regionCount;
    const board = state.boards.boards.get(rc);
    if (!board){
        return;
    }

    let bv = BoardViewFactory.createEmptyBoardView(board);
    bv = BoardViewFactory.fillEmptyBoardView(bv, game);
    const data : CanvasTranformData = {
        containerSize: state.display.boardContainerSize,
        canvasMargin: state.display.canvasMargin,
        contentPadding: state.display.canvasContentPadding,
        zoomLevel: state.display.boardZoomLevel,
        regionCount: state.activeGame.game.parameters.regionCount
    };
    const t = CanvasTransformService.getBoardViewTransform(data);
    bv = Geometry.Board.transform(bv, t);
    state.activeGame.boardView = bv;
}

//#endregion

function setNavigationOptionsReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<NavigationState>>action;
    const newState = {...state};
    newState.navigation = da.data;
    return newState;
}

function boardZoomReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<number>>action;
    const newState = {...state};
    newState.display = {...state.display};
    newState.display.boardZoomLevel = da.data;
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function boardScrollReducer(state: AppState, action: CustomAction) : AppState {
    const da = <DataAction<Point>>action;
    const newState = {...state};
    newState.display = {...state.display};
    newState.display.boardScrollPercent = da.data;
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function loadPieceImageReducer(state : AppState, action : CustomAction) : AppState {
    const da = <DataAction<[PieceKind, HTMLImageElement]>>action;
    const [kind, image] = da.data;
    const newState = {...state};
    newState.images = {...state.images};
    const pieces = new Map<PieceKind, HTMLImageElement>(state.images.pieces);
    pieces.set(kind, image);
    newState.images.pieces = pieces;
    return newState;
}

function apiRequestReducer(state : AppState, action : CustomAction) : AppState {
    const da = <DataAction<ApiRequest>>action;
    const newState = {...state};
    newState.apiClient = {...state.apiClient};
    newState.apiClient.openRequests.push(da.data);
    return newState;
}

function apiResponseReducer(state : AppState, action : CustomAction) : AppState {
    const da = <DataAction<ApiResponse>>action;
    const newState = {...state};
    newState.apiClient = {...state.apiClient};
    newState.apiClient.openRequests = state.apiClient.openRequests.filter(r => r.id !== da.data.requestId);
    return newState;
}

function apiErrorReducer(state : AppState, action : CustomAction) : AppState {
    const da = <DataAction<ApiError>>action;
    const newState = {...state};
    newState.apiClient = {...state.apiClient};
    newState.apiClient.openRequests = state.apiClient.openRequests.filter(r => r.id !== da.data.requestId);
    return newState;
}
