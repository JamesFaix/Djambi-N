import { User, Game, GamesQuery, GameParameters, Event, Board, PieceKind } from "../api/model";
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
  AddPlayer = "ADD_PLAYER",
  RemovePlayer = "REMOVE_PLAYER",
  StartGame = "START_GAME",
  SetNavigationOptions = "SET_NAV_OPTIONS",
  LoadBoard = "LOAD_BOARD",
  SelectCell = "SELECT_CELL",
  BoardZoom = "BOARD_ZOOM",
  BoardScroll = "BOARD_SCROLL",
  LoadPieceImage = "LOAD_PIECE_IMAGE",
  EndTurn = "END_TURN",
  ResetTurn = "RESET_TURN",
  ChangePlayerStatus = "CHANGE_PLAYER_STATUS",
  ApiRequest = "API_REQUEST",
  ApiResponse = "API_RESPONSE",
  ApiError = "API_ERROR"
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

export const addPlayer = (game : Game) => create(ActionTypes.AddPlayer, game);
export const removePlayer = (game : Game) => create(ActionTypes.RemovePlayer, game);
export const startGame = (game : Game) => create(ActionTypes.StartGame, game);
export const selectCell = (game : Game) => create(ActionTypes.SelectCell, game);
export const endTurn = (game : Game) => create(ActionTypes.EndTurn, game);
export const resetTurn = (game : Game) => create(ActionTypes.ResetTurn, game);
export const changePlayerStatus = (game : Game) => create(ActionTypes.ChangePlayerStatus, game);

export const setNavigationOptions = (options : NavigationState) => create(ActionTypes.SetNavigationOptions, options);
export const boardZoom = (level : number) => create(ActionTypes.BoardZoom, level);
export const boardScroll = (scrollPercent : Point) => create(ActionTypes.BoardScroll, scrollPercent);
export const loadPieceImage = (pieceKind : PieceKind, image : HTMLImageElement) => create(ActionTypes.LoadPieceImage, [pieceKind, image]);

export const apiRequest = (request : ApiRequest) => create(ActionTypes.ApiRequest, request);
export const apiResponse = (response : ApiResponse) => create(ActionTypes.ApiResponse, response);
export const apiError = (apiError : ApiError) => create(ActionTypes.ApiError, apiError);