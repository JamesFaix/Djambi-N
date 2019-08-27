import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GamesQuery, GameParameters, CreatePlayerRequest, User, GameStatus, Game, EventsQuery, Event, ResultsDirection, Board, PlayerStatus } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";
import { navigateTo } from './history';

//#region Session actions

export function login(request : LoginRequest) {
    return async function (dispatch : Dispatch) : Promise<void> {
        const session = await Api.login(request);
        dispatch(Actions.login(session.user));
        navigateTo(Routes.dashboard);
        return queryGamesForUser(session.user, dispatch);
    };
}

export function logout() {
    return async function (dispatch : Dispatch) : Promise<void> {
        await Api.logout();
        dispatch(Actions.logout());
        navigateTo(Routes.login);
    };
}

export function signup(request : CreateUserRequest) {
    return async function (dispatch : Dispatch) : Promise<void> {
        const user = await Api.createUser(request);
        dispatch(Actions.signup(user));
        const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
        return login(loginRequest)(dispatch);
    };
}

export function restoreSession() {
    return async function (dispatch : Dispatch) : Promise<void> {
        try {
            const user = await Api.getCurrentUser();
            dispatch(Actions.restoreSession(user));
            return queryGamesForUser(user, dispatch);
        }
        catch (ex) {
            let [status, message] = ex;
            if (status !== 401) {
                throw ex;
            }
        }
    };
}

export function redirectToDashboardIfLoggedIn() {
    return async function (dispatch : Dispatch) : Promise<void> {
        try {
            const user = await Api.getCurrentUser();
                dispatch(Actions.restoreSession(user));
                navigateTo(Routes.dashboard);
                return queryGamesForUser(user, dispatch);
        }
        catch(ex) {
            let [status, message] = ex;
            if (status !== 401) {
                throw ex;
            }
        }
    };
}

export function redirectToLoginIfNotLoggedIn() {
    return async function (dispatch : Dispatch) : Promise<void> {
        try {
            const user = await Api.getCurrentUser();
            dispatch(Actions.restoreSession(user))
            return queryGamesForUser(user, dispatch);
        }
        catch (ex) {
            let [status, message] = ex;
            if (status !== 401) {
                throw ex;
            }
            navigateTo(Routes.login);
        }
    };
}

export function redirectToLoginOrDashboard() {
    return async function (dispatch : Dispatch) : Promise<void> {
        try {
            const user = await Api.getCurrentUser();
            dispatch(Actions.restoreSession(user));
            navigateTo(Routes.dashboard);
            return queryGamesForUser(user, dispatch);
        }
        catch (ex) {
            let [status, message] = ex;
            if (status === 401) {
                navigateTo(Routes.login);
            }
            else {
                throw ex;
            }
        }
    };
}

//#endregion

//#region Game actions

async function loadGameInner(gameId : number, dispatch : Dispatch) : Promise<Game> {
    const game = await Api.getGame(gameId);
    dispatch(Actions.loadGame(game));
    return game;
}

async function loadHistoryInner(gameId : number, dispatch : Dispatch) : Promise<Event[]> {
    const query : EventsQuery = {
        maxResults : null,
        direction : ResultsDirection.Descending,
        thresholdTime : null,
        thresholdEventId : null
    };

    const history = await Api.getEvents(gameId, query);
    dispatch(Actions.loadGameHistory(history));
    return history;
}

async function loadBoardInner(regionCount : number, dispatch : Dispatch) : Promise<Board> {
    const board = await Api.getBoard(regionCount);
    dispatch(Actions.loadBoard(board));
    return board;
}

export function loadGame(gameId: number) {
    return async function (dispatch : Dispatch) : Promise<void> {
        await loadGameInner(gameId, dispatch);
    };
}

export function loadGameFull(gameId : number) {
    return async function (dispatch : Dispatch) : Promise<void> {
        const game = await loadGameInner(gameId, dispatch);
        await loadHistoryInner(gameId, dispatch);
        await loadBoardInner(game.parameters.regionCount, dispatch);
    };
}

export function createGame(formData : GameParameters) {
    return async function (dispatch : Dispatch) : Promise<void> {
        const game = await Api.createGame(formData);
        dispatch(Actions.createGame(game));
        navigateTo(Routes.lobby(game.id));
    }
}

export function addPlayer(gameId: number, request : CreatePlayerRequest) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.addPlayer(gameId, request);
        dispatch(Actions.updateGame(resp));
    }
}

export function removePlayer(gameId: number, playerId: number) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.removePlayer(gameId, playerId);
        dispatch(Actions.updateGame(resp));
    }
}

export function startGame(gameId: number) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.startGame(gameId);
        dispatch(Actions.updateGame(resp));
        navigateTo(Routes.play(gameId));
    }
}

export function selectCell(gameId: number, cellId : number) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.selectCell(gameId, cellId);
        dispatch(Actions.updateGame(resp));
    }
}

export function endTurn(gameId: number) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.commitTurn(gameId);
        dispatch(Actions.updateGame(resp));
    }
}

export function resetTurn(gameId: number) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.resetTurn(gameId);
        dispatch(Actions.updateGame(resp));
    }
}

export function changePlayerStatus(gameId: number, playerId: number, status: PlayerStatus) {
    return async function (dispatch: Dispatch) : Promise<void> {
        const resp = await Api.updatePlayerStatus(gameId, playerId, status);
        dispatch(Actions.updateGame(resp));
    }
}

//#endregion

//#region Games search

export function queryGames(query: GamesQuery) {
    return async function (dispatch : Dispatch) : Promise<void> {
        const games = await Api.getGames(query);
        dispatch(Actions.queryGames(games));
    };
}

//#endregion

//#region Helper functions

function queryGamesForUser(user: User, dispatch: Dispatch) : Promise<void> {
    const query = ModelFactory.emptyGamesQuery();
    query.playerUserName = user.name;
    dispatch(Actions.updateGamesQuery(query));
    return queryGames(query)(dispatch)
}

export function navigateToGame(game : Game) : void {
    const route = game.status === GameStatus.InProgress
        ? Routes.play(game.id)
        : Routes.lobby(game.id);
    navigateTo(route);
}

//#endregion