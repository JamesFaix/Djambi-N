import { Session, Game, LoginRequest, Event, User, CreateUserRequest } from "../api/model";
import { AnyAction } from "redux";

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
  UpdateGame = "UPDATE_GAME",
  LoadHistory = "LOAD_HISTORY"
}

export interface CustomAction {
  type: ActionTypes,
  status: ActionStatus,
}

export interface DataAction<T> extends CustomAction {
  data : T
}

export class ActionFactory {
  constructor() {

  }

  loginRequest(request: LoginRequest) : DataAction<LoginRequest> {
    return {
      type: ActionTypes.Login,
      status: ActionStatus.Pending,
      data: request
    };
  }

  loginSuccess(session : Session) : DataAction<Session> {
    return {
      type: ActionTypes.Login,
      status: ActionStatus.Success,
      data: session
    };
  }

  loginError() : CustomAction {
    return {
      type: ActionTypes.Login,
      status: ActionStatus.Error
    };
  }

  logoutRequest() : CustomAction {
    return {
      type: ActionTypes.Logout,
      status: ActionStatus.Pending
    };
  }

  logoutSuccess() : CustomAction {
    return {
      type: ActionTypes.Logout,
      status: ActionStatus.Success
    };
  }
  
  logoutError() : CustomAction {
    return {
      type: ActionTypes.Logout,
      status: ActionStatus.Error
    };
  }

  signupRequest(request : CreateUserRequest) : DataAction<CreateUserRequest> {
    return {
      type: ActionTypes.Signup,
      status: ActionStatus.Pending,
      data: request
    };
  }

  signupSuccess(user : User) : DataAction<User> {
    return {
      type: ActionTypes.Signup,
      status: ActionStatus.Success,
      data: user
    };
  }

  signupError() : CustomAction {
    return {
      type: ActionTypes.Signup,
      status: ActionStatus.Error
    };
  }

  loadGameRequest(gameId : number) : DataAction<number> {
    return {
      type: ActionTypes.LoadGame,
      status: ActionStatus.Pending,
      data: gameId
    };
  }

  loadGameSuccess(game : Game) : DataAction<Game> {
    return {
      type: ActionTypes.LoadGame,
      status: ActionStatus.Success,
      data: game
    };
  }

  loadGameError() : CustomAction {
    return {
      type: ActionTypes.LoadGame,
      status: ActionStatus.Error
    };
  }

  loadGameHistoryRequest(gameId : number) : DataAction<number> {
    return {
      type: ActionTypes.LoadHistory,
      status: ActionStatus.Pending,
      data: gameId
    };
  }

  loadGameHistorySuccess(history : Event[]) : DataAction<Event[]> {
    return {
      type: ActionTypes.LoadHistory,
      status: ActionStatus.Success,
      data: history
    };
  }

  loadGameHistoryError() : CustomAction {
    return {
      type: ActionTypes.LoadHistory,
      status: ActionStatus.Error
    };
  }

  updateGame(game : Game, event : Event) : DataAction<[Game, Event]> {
    return {
      type: ActionTypes.UpdateGame,
      status: ActionStatus.Success,
      data: [game, event]
    };
  }
}
