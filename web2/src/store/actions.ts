import { Session, LoginRequest, User, CreateUserRequest, Game, GamesQuery } from "../api/model";

export enum ActionStatus {
  Pending = "PENDING",
  Success = "SUCCESS",
  Error = "ERROR"
}

export enum ActionTypes {
  Login = "LOGIN",
  Logout = "LOGOUT",
  Signup = "SIGNUP",
  LoadGame = "LOAD_GAME",
  QueryGames = "QUERY_GAMES",
  UpdateGamesQuery = "UPDATE_GAMES_QUERY",
  Redirect = "REDIRECT"
}

export interface CustomAction {
  type: ActionTypes,
  status: ActionStatus,
}

export interface DataAction<T> extends CustomAction {
  data : T
}

export function loginRequest(request: LoginRequest) : DataAction<LoginRequest> {
  return {
    type: ActionTypes.Login,
    status: ActionStatus.Pending,
    data: request
  };
}

export function loginSuccess(session : Session) : DataAction<Session> {
  return {
    type: ActionTypes.Login,
    status: ActionStatus.Success,
    data: session
  };
}

export function loginError() : CustomAction {
  return {
    type: ActionTypes.Login,
    status: ActionStatus.Error
  };
}

export function logoutRequest() : CustomAction {
  return {
    type: ActionTypes.Logout,
    status: ActionStatus.Pending
  };
}

export function logoutSuccess() : CustomAction {
  return {
    type: ActionTypes.Logout,
    status: ActionStatus.Success
  };
}

export function logoutError() : CustomAction {
  return {
    type: ActionTypes.Logout,
    status: ActionStatus.Error
  };
}

export function signupRequest(request : CreateUserRequest) : DataAction<CreateUserRequest> {
  return {
    type: ActionTypes.Signup,
    status: ActionStatus.Pending,
    data: request
  };
}

export function signupSuccess(user : User) : DataAction<User> {
  return {
    type: ActionTypes.Signup,
    status: ActionStatus.Success,
    data: user
  };
}

export function signupError() : CustomAction {
  return {
    type: ActionTypes.Signup,
    status: ActionStatus.Error
  };
}

export function loadGameRequest(gameId : number) : DataAction<number> {
  return {
    type: ActionTypes.LoadGame,
    status: ActionStatus.Pending,
    data: gameId
  };
}

export function loadGameSuccess(game : Game) : DataAction<Game> {
  return {
    type: ActionTypes.LoadGame,
    status: ActionStatus.Success,
    data: game
  };
}

export function loadGameError() : CustomAction {
  return {
    type: ActionTypes.LoadGame,
    status: ActionStatus.Error
  };
}

export function queryGamesRequest(query : GamesQuery) : DataAction<GamesQuery> {
  return {
    type: ActionTypes.QueryGames,
    status: ActionStatus.Pending,
    data: query
  };
}

export function queryGamesSuccess(games : Game[]) : DataAction<Game[]> {
  return {
    type: ActionTypes.QueryGames,
    status: ActionStatus.Success,
    data: games
  };
}

export function queryGamesError() : CustomAction {
  return {
    type: ActionTypes.QueryGames,
    status: ActionStatus.Error
  };
}

export function updateGamesQuery(query: GamesQuery) : DataAction<GamesQuery> {
  return {
    type: ActionTypes.UpdateGamesQuery,
    status: ActionStatus.Success,
    data: query
  };
}

export function redirectPending(route : string) : DataAction<string> {
  return {
    type: ActionTypes.Redirect,
    status: ActionStatus.Pending,
    data: route
  };
}

export function redirectSuccess() : CustomAction {
  return {
    type: ActionTypes.Redirect,
    status: ActionStatus.Success
  };
}