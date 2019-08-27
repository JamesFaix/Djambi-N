import { User, Game, GamesQuery, GameParameters, Event, Board, PieceKind, StateAndEventResponse } from "../api/model";
import { NavigationState } from "./state";
import { Point } from "../viewModel/board/model";
import { ApiResponse, ApiRequest, ApiError } from "../api/requestModel";

export enum ActionTypes {
  Login = "LOGIN",
  Logout = "LOGOUT",
  Signup = "SIGNUP",
  LoadGame = "LOAD_GAME",
  LoadGameHistory = "LOAD_GAME_HISTORY",
  QueryGames = "QUERY_GAMES",
  UpdateGamesQuery = "UPDATE_GAMES_QUERY",
  RestoreSession = "RESTORE_SESSION",
  CreateGame = "CREATE_GAME",
  UpdateCreateGameForm = "UPDATE_CREATE_GAME_FORM",
  SetNavigationOptions = "SET_NAV_OPTIONS",
  LoadBoard = "LOAD_BOARD",
  BoardZoom = "BOARD_ZOOM",
  BoardScroll = "BOARD_SCROLL",
  LoadPieceImage = "LOAD_PIECE_IMAGE",
  ApiRequest = "API_REQUEST",
  ApiResponse = "API_RESPONSE",
  ApiError = "API_ERROR",
  UpdateGame = "UPDATE_GAME"
}

export interface CustomAction {
  type: ActionTypes
}

export interface DataAction<T> extends CustomAction {
  data : T
}

function create<T>(type : ActionTypes, data : T = undefined) {
  return {
    type: type,
    data: data
  };
}

export const login = (user : User) => create(ActionTypes.Login, user);
export const logout = () => create(ActionTypes.Logout);
export const signup = (user : User) => create(ActionTypes.Signup, user);
export const restoreSession = (user: User) => create(ActionTypes.RestoreSession, user);

export const loadGame = (game : Game) => create(ActionTypes.LoadGame, game);
export const loadGameHistory = (history : Event[]) => create(ActionTypes.LoadGameHistory, history);
export const loadBoard = (board : Board) => create(ActionTypes.LoadBoard, board);

export const queryGames = (games : Game[]) => create(ActionTypes.QueryGames, games);
export const updateGamesQuery = (query: GamesQuery) => create(ActionTypes.UpdateGamesQuery, query);

export const updateCreateGameForm = (parameters : GameParameters) => create(ActionTypes.UpdateCreateGameForm, parameters);
export const createGame = (game : Game) => create(ActionTypes.CreateGame, game);

export const updateGame = (response : StateAndEventResponse) => create(ActionTypes.UpdateGame, response);

export const setNavigationOptions = (options : NavigationState) => create(ActionTypes.SetNavigationOptions, options);
export const boardZoom = (level : number) => create(ActionTypes.BoardZoom, level);
export const boardScroll = (scrollPercent : Point) => create(ActionTypes.BoardScroll, scrollPercent);
export const loadPieceImage = (pieceKind : PieceKind, image : HTMLImageElement) => create(ActionTypes.LoadPieceImage, [pieceKind, image]);

export const apiRequest = (request : ApiRequest) => create(ActionTypes.ApiRequest, request);
export const apiResponse = (response : ApiResponse) => create(ActionTypes.ApiResponse, response);
export const apiError = (apiError : ApiError) => create(ActionTypes.ApiError, apiError);