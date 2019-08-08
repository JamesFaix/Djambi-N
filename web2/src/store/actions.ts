import { Session, LoginRequest, User, CreateUserRequest } from "../api/model";

export enum ActionStatus {
  Pending = "PENDING",
  Success = "SUCCESS",
  Error = "ERROR"
}

export enum ActionTypes {
  Login = "LOGIN",
  Logout = "LOGOUT",
  Signup = "SIGNUP"
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