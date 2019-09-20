import * as Search from './search';
import * as CreateGameForm from './createGameForm';
import * as Display from './display';
import * as Session from './session';
import * as ActiveGame from './activeGame';
import * as Boards from './boards';
import * as ApiClient from './apiClient';
import * as Notifications from './notifications';
import { combineReducers, Reducer, Store } from 'redux';
import { Game, Board, StateAndEventResponse } from '../api/model';
import CanvasTransformService, { CanvasTranformData } from '../viewModel/board/canvasTransformService';
import BoardViewFactory from '../viewModel/board/boardViewFactory';
import Geometry from '../viewModel/board/geometry';
import { Point } from '../viewModel/board/model';
import GameHistory from '../viewModel/gameHistory';
import * as Settings from './settings';
import { MapUtil } from '../utilities/collections';

export interface State {
    session: Session.State,
    activeGame : ActiveGame.State,
    search : Search.State,
    createGameForm : CreateGameForm.State,
    boards : Boards.State,
    display : Display.State,
    apiClient : ApiClient.State,
    settings : Settings.State,
    notifications : Notifications.State
}

export const defaultState : State = {
    session: Session.defaultState,
    activeGame: ActiveGame.defaultState,
    search: Search.defaultState,
    createGameForm: CreateGameForm.defaultState,
    boards: Boards.defaultState,
    display: Display.defaultState,
    apiClient: ApiClient.defaultState,
    settings: Settings.defaultState,
    notifications: Notifications.defaultState
}

export interface CustomAction {
    type: string
}

export interface DataAction<T> extends CustomAction {
    data : T
}

export type AppStore = Store<State, CustomAction>;

export function getAppState(store : AppStore) : State {
    return store.getState() as State;
}

const combinedReducer : Reducer<State, CustomAction> = combineReducers({
    activeGame: ActiveGame.reducer,
    apiClient: ApiClient.reducer,
    boards: Boards.reducer,
    createGameForm : CreateGameForm.reducer,
    display: Display.reducer,
    search: Search.reducer,
    session: Session.reducer,
    settings: Settings.reducer,
    notifications: Notifications.reducer
});

export function reducer(state: State, action : CustomAction) : State {
    const newState = reducerInner(state, action);
    if (newState.settings.debug && newState.settings.debug.logRedux) {
        console.log("Old state:");
        console.log(state);
        console.log(action);
        console.log("New state:");
        console.log(newState);
    }
    return newState;
}

function reducerInner(state: State, action : CustomAction) : State {
    switch (action.type) {
        case Session.ActionTypes.Logout:
            return logoutReducer(state, action);
        case ActiveGame.ActionTypes.LoadGame:
            return loadGameReducer(state, action);
        case Boards.ActionTypes.LoadBoard:
            return loadBoardReducer(state, action);
        case ActiveGame.ActionTypes.UpdateGame:
            return updateGameReducer(state, action);
        case Display.ActionTypes.BoardZoom:
            return boardZoomReducer(state, action);
        case Display.ActionTypes.BoardScroll:
            return boardScrollReducer(state, action);
        case Display.ActionTypes.BoardAreaResize:
            return boardResizeReducer(state, action);
        default:
            return combinedReducer(state, action);
    }
}

//#region Game actions

function loadGameReducer(state: State, action: CustomAction) : State {
    const da = <DataAction<Game>>action;
    const newState = {...state};
    newState.activeGame.game = da.data;
    updateBoardView(newState, da.data);
    return newState;
}

function loadBoardReducer(state: State, action: CustomAction) : State {
    const da = <DataAction<Board>>action;
    const newState : State = {
        ...state,
        boards: {
            ...state.boards,
            boards: MapUtil.add(state.boards.boards, da.data.regionCount, da.data)
        }
    };
    //There won't be an active game if boards are being loaded for thumbnails
    if (state.activeGame.game) {
        updateBoardView(newState, state.activeGame.game);
    }
    return newState;
}

function updateGameReducer(state: State, action : CustomAction) : State {
    const da = <DataAction<StateAndEventResponse>>action;
    const game = da.data.game;
    const event = da.data.event;
    const newState = {...state};
    newState.activeGame = {...state.activeGame};
    newState.activeGame.game = game;
    if (GameHistory.isDisplayableEvent(event)) {
        newState.activeGame.history = [event].concat(state.activeGame.history);
    }
    updateBoardView(newState, game);
    return newState;
}

function updateBoardView(state : State, game : Game) : void {
    const rc = game.parameters.regionCount;
    const board = state.boards.boards.get(rc);
    if (!board){
        return;
    }

    let bv = BoardViewFactory.createEmptyBoardView(board);
    bv = BoardViewFactory.fillEmptyBoardView(bv, game, state.session.user);
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

function boardZoomReducer(state: State, action: CustomAction) : State {
    const da = <DataAction<number>>action;
    const newState = {...state};
    newState.display = {...state.display};
    newState.display.boardZoomLevel = da.data;
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function boardScrollReducer(state: State, action: CustomAction) : State {
    const da = <DataAction<Point>>action;
    const newState = {...state};
    newState.display = {...state.display};
    newState.display.boardScrollPercent = da.data;
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function boardResizeReducer(state : State, action : CustomAction) : State {
    const da = <DataAction<Point>>action;
    const newState = {
        ...state,
        display: {
            ...state.display,
            boardContainerSize: da.data
        }
    };
    updateBoardView(newState, state.activeGame.game);
    return newState;
}

function logoutReducer(state: State, action: CustomAction) : State {
    return {
        ...defaultState,
        boards: state.boards,
        display: {
            ...defaultState.display,
            images: {
                ...defaultState.display.images,
                pieces: state.display.images.pieces
            },
            theme: state.display.theme
        },
        settings: state.settings
    };
}