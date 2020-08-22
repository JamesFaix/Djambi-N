import { SessionActionTypes } from './actionTypes';
import { UserDto } from '../../api-client';

export type LoggedInAction = {
  type: typeof SessionActionTypes.LoggedIn,
  user: UserDto
};

export type LoggedOutAction = {
  type: typeof SessionActionTypes.LoggedOut,
};

export type AttemptingRestoreAction = {
  type: typeof SessionActionTypes.AttemptingRestore,
};

export type RestoreSucceededAction = {
  type: typeof SessionActionTypes.RestoreSucceeded,
  user: UserDto
};

export type RestoreFailedAction = {
  type: typeof SessionActionTypes.RestoreFailed,
};

export type SessionAction =
  LoggedInAction |
  LoggedOutAction |
  AttemptingRestoreAction |
  RestoreSucceededAction |
  RestoreFailedAction;
