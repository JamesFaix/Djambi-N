import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, GameParameters, CreatePlayerRequest, User, GameStatus, Game, EventsQuery, Event, ResultsDirection, Board, PlayerStatus, GamesQuery, CreateSnapshotRequest } from "./api/model";
import * as Api from "./api/client";
import * as ModelFactory from "./api/modelFactory";
import Routes from "./routes";
import { navigateTo } from './history';
import * as StoreGamesQuery from './store/gamesQuery';
import * as StoreSession from './store/session';
import * as StoreActiveGame from './store/activeGame';
import * as StoreBoards from './store/boards';
import { SseClientManager } from "./utilities/serverSentEvents";
import ThemeService from "./themes/themeService";
import ThemeFactory from "./themes/themeFactory";

//#region Session actions

export default class ApiActions {
    public static login(request : LoginRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const session = await Api.login(request);
            dispatch(StoreSession.Actions.login(session.user));
            navigateTo(Routes.dashboard);
            return ApiActions.finishLoginSetup(session.user, dispatch);
        };
    }

    public static logout() {
        return async function (dispatch : Dispatch) : Promise<void> {
            await Api.logout();
            dispatch(StoreSession.Actions.logout());
            navigateTo(Routes.login);
            SseClientManager.disconnect();
        };
    }

    public static signup(request : CreateUserRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const user = await Api.createUser(request);
            dispatch(StoreSession.Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return ApiActions.login(loginRequest)(dispatch);
        };
    }

    public static redirectToDashboardIfLoggedIn() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(StoreSession.Actions.restoreSession(user));
                navigateTo(Routes.dashboard);
                return ApiActions.finishLoginSetup(user, dispatch);
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
                dispatch(StoreSession.Actions.restoreSession(user));
                return ApiActions.finishLoginSetup(user, dispatch);
            }
            catch (ex) {
                console.log(ex);
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
                dispatch(StoreSession.Actions.restoreSession(user));
                navigateTo(Routes.dashboard);
                return ApiActions.finishLoginSetup(user, dispatch);
            }
            catch (ex) {
                console.log(ex);
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

    private static finishLoginSetup(user : User, dispatch : Dispatch) : Promise<void> {
        SseClientManager.connect();
        ThemeService.changeTheme(ThemeFactory.default.name, dispatch); //TODO: Use user-preferred theme
        return ApiActions.queryGamesForUser(user, dispatch);
    }

    //#endregion

    //#region Game actions

    private static async loadGameInner(gameId : number, dispatch : Dispatch) : Promise<Game> {
        const game = await Api.getGame(gameId);
        dispatch(StoreActiveGame.Actions.loadGame(game));
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
        dispatch(StoreActiveGame.Actions.loadGameHistory(history));
        return history;
    }

    private static async loadBoardInner(regionCount : number, dispatch : Dispatch) : Promise<Board> {
        const board = await Api.getBoard(regionCount);
        dispatch(StoreBoards.Actions.loadBoard(board));
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
            dispatch(StoreActiveGame.Actions.createGame(game));
            navigateTo(Routes.lobby(game.id));
        }
    }

    public static addPlayer(gameId: number, request : CreatePlayerRequest) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.addPlayer(gameId, request);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    public static removePlayer(gameId: number, playerId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.removePlayer(gameId, playerId);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    public static startGame(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.startGame(gameId);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
            navigateTo(Routes.play(gameId));
        }
    }

    public static selectCell(gameId: number, cellId : number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.selectCell(gameId, cellId);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    public static endTurn(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.commitTurn(gameId);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    public static resetTurn(gameId: number) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.resetTurn(gameId);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    public static changePlayerStatus(gameId: number, playerId: number, status: PlayerStatus) {
        return async function (dispatch: Dispatch) : Promise<void> {
            const resp = await Api.updatePlayerStatus(gameId, playerId, status);
            dispatch(StoreActiveGame.Actions.updateGame(resp));
        }
    }

    //#endregion

    //#region Snapshots

    public static getSnapshots(gameId: number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            const snapshots = await Api.getSnapshotsForGame(gameId);
            dispatch(StoreActiveGame.Actions.loadSnapshots(snapshots))
        }
    }

    public static saveSnapshot(gameId : number, request : CreateSnapshotRequest) {
        return async function(dispatch: Dispatch) : Promise<void> {
            const snapshot = await Api.createSnapshot(gameId, request);
            dispatch(StoreActiveGame.Actions.snapshotSaved(snapshot));
        }
    }

    public static loadSnapshot(gameId : number, snapshotId : number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            return ApiActions.loadGameFull(gameId)(dispatch);
        }
    }

    public static deleteSnapshot(gameId : number, snapshotId : number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            dispatch(StoreActiveGame.Actions.snapshotDeleted(snapshotId));
        }
    }

    //#endregion

    //#region Games search

    public static queryGames(query: GamesQuery) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const games = await Api.getGames(query);
            dispatch(StoreGamesQuery.Actions.queryGames(games));
        };
    }

    //#endregion

    //#region Helper functions

    private static queryGamesForUser(user: User, dispatch: Dispatch) : Promise<void> {
        const query = ModelFactory.emptyGamesQuery();
        query.playerUserName = user.name;
        dispatch(StoreGamesQuery.Actions.updateGamesQuery(query));
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

