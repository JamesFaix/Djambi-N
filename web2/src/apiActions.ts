import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GamesQuery, GameParameters, CreatePlayerRequest, User, GameStatus, Game, EventsQuery, Event, ResultsDirection, Board, PlayerStatus } from "./api/model";
import * as Actions from "./store/actions";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";
import { navigateTo } from './history';

//#region Session actions

export default class ApiActions {
    public static login(request : LoginRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const session = await Api.login(request);
            dispatch(Actions.login(session.user));
            navigateTo(Routes.dashboard);
            return ApiActions.queryGamesForUser(session.user, dispatch);
        };
    }

    public static logout() {
        return async function (dispatch : Dispatch) : Promise<void> {
            await Api.logout();
            dispatch(Actions.logout());
            navigateTo(Routes.login);
        };
    }

    public static signup(request : CreateUserRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const user = await Api.createUser(request);
            dispatch(Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return ApiActions.login(loginRequest)(dispatch);
        };
    }

    public static restoreSession() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(Actions.restoreSession(user));
                return ApiActions.queryGamesForUser(user, dispatch);
            }
            catch (ex) {
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
            }
        };
    }

    public static redirectToDashboardIfLoggedIn() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                    dispatch(Actions.restoreSession(user));
                    navigateTo(Routes.dashboard);
                    return ApiActions.queryGamesForUser(user, dispatch);
            }
            catch(ex) {
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
            }
        };
    }

    public static redirectToLoginIfNotLoggedIn() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(Actions.restoreSession(user))
                return ApiActions.queryGamesForUser(user, dispatch);
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

    public static redirectToLoginOrDashboard() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(Actions.restoreSession(user));
                navigateTo(Routes.dashboard);
                return ApiActions.queryGamesForUser(user, dispatch);
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

    private static async loadGameInner(gameId : number, dispatch : Dispatch) : Promise<Game> {
        const game = await Api.getGame(gameId);
        dispatch(Actions.loadGame(game));
        return game;
    }

    private static async loadHistoryInner(gameId : number, dispatch : Dispatch) : Promise<Event[]> {
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

    private static async loadBoardInner(regionCount : number, dispatch : Dispatch) : Promise<Board> {
        const board = await Api.getBoard(regionCount);
        dispatch(Actions.loadBoard(board));
        return board;
    }

    public static loadGame(gameId: number) {
        return async function (dispatch : Dispatch) : Promise<void> {
            await ApiActions.loadGameInner(gameId, dispatch);
        };
    }

    public static loadGameFull(gameId : number) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const game = await ApiActions.loadGameInner(gameId, dispatch);
            await ApiActions.loadHistoryInner(gameId, dispatch);
            await ApiActions.loadBoardInner(game.parameters.regionCount, dispatch);
        };
    }

    public static createGame(formData : GameParameters) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const game = await Api.createGame(formData);
            dispatch(Actions.createGame(game));
            navigateTo(Routes.lobby(game.id));
        }
    }

    public static addPlayer(gameId: number, request : CreatePlayerRequest) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.addPlayer(gameId, request);
            dispatch(Actions.updateGame(resp));
        }
    }

    public static removePlayer(gameId: number, playerId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.removePlayer(gameId, playerId);
            dispatch(Actions.updateGame(resp));
        }
    }

    public static startGame(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.startGame(gameId);
            dispatch(Actions.updateGame(resp));
            navigateTo(Routes.play(gameId));
        }
    }

    public static selectCell(gameId: number, cellId : number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.selectCell(gameId, cellId);
            dispatch(Actions.updateGame(resp));
        }
    }

    public static endTurn(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.commitTurn(gameId);
            dispatch(Actions.updateGame(resp));
        }
    }

    public static resetTurn(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.resetTurn(gameId);
            dispatch(Actions.updateGame(resp));
        }
    }

    public static changePlayerStatus(gameId: number, playerId: number, status: PlayerStatus) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.updatePlayerStatus(gameId, playerId, status);
            dispatch(Actions.updateGame(resp));
        }
    }

    //#endregion

    //#region Games search

    public static queryGames(query: GamesQuery) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const games = await Api.getGames(query);
            dispatch(Actions.queryGames(games));
        };
    }

    //#endregion

    //#region Helper functions

    private static queryGamesForUser(user: User, dispatch: Dispatch) : Promise<void> {
        const query = ModelFactory.emptyGamesQuery();
        query.playerUserName = user.name;
        dispatch(Actions.updateGamesQuery(query));
        return ApiActions.queryGames(query)(dispatch)
    }

    public static navigateToGame(game : Game) : void {
        const route = game.status === GameStatus.InProgress
            ? Routes.play(game.id)
            : Routes.lobby(game.id);
        navigateTo(route);
    }

    //#endregion
}

