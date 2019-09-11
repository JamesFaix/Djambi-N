import * as StoreActiveGame from '../store/activeGame';
import { Dispatch } from 'redux';
import * as Api from "../api/client";
import {
    Game,
    EventsQuery,
    ResultsDirection,
    Event,
    Board,
    GameParameters,
    CreatePlayerRequest,
    PlayerStatus,
    GameStatus,
    StateAndEventResponse
} from '../api/model';
import Routes from '../routes';
import * as StoreBoards from '../store/boards';
import Controller from './controller';

export default class GameStoreFlows {
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

    public static loadAllBoards() {
        return async function(dispatch : Dispatch) : Promise<void> {
            for (let i=3; i<=8; i++){
                await GameStoreFlows.loadBoardInner(i, dispatch);
            }
        };
    }

    private static async loadBoardInner(regionCount : number, dispatch : Dispatch) : Promise<Board> {
        const board = await Api.getBoard(regionCount);
        dispatch(StoreBoards.Actions.loadBoard(board));
        return board;
    }

    public static loadGame(gameId: number) {
        return async function (dispatch : Dispatch) : Promise<void> {
            await GameStoreFlows.loadGameInner(gameId, dispatch);
        };
    }

    public static loadGameFull(gameId : number) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const game = await GameStoreFlows.loadGameInner(gameId, dispatch);
            await GameStoreFlows.loadHistoryInner(gameId, dispatch);
            await GameStoreFlows.loadBoardInner(game.parameters.regionCount, dispatch);
        };
    }

    public static createGame(formData : GameParameters) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const game = await Api.createGame(formData);
            dispatch(StoreActiveGame.Actions.createGame(game));
            Controller.navigateTo(Routes.lobby(game.id));
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
            Controller.navigateTo(Routes.play(gameId));
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
            GameStoreFlows.updateInProgressGame(resp, dispatch);
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
            GameStoreFlows.updateInProgressGame(resp, dispatch);
        }
    }

    public static updateInProgressGame(response : StateAndEventResponse, dispatch : Dispatch) : void {
        dispatch(StoreActiveGame.Actions.updateGame(response));
        const g = response.game;
        if (g.status === GameStatus.Over) {
            Controller.navigateTo(Routes.gameOver(g.id));
        }
    }
}