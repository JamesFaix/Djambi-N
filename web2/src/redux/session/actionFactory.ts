import { UserDto } from '../../api-client';
import {
  LoggedInAction,
  LoggedOutAction,
  AttemptingRestoreAction,
  RestoreSucceededAction,
  RestoreFailedAction,
} from './actions';
import { SessionActionTypes } from './actionTypes';

export function loggedIn(user: UserDto): LoggedInAction {
  return {
    type: SessionActionTypes.LoggedIn,
    user,
  };
}

export function loggedOut(): LoggedOutAction {
  return {
    type: SessionActionTypes.LoggedOut,
  };
}

export function attemptingRestore(): AttemptingRestoreAction {
  return {
    type: SessionActionTypes.AttemptingRestore,
  };
}

export function restoreSucceeded(user: UserDto): RestoreSucceededAction {
  return {
    type: SessionActionTypes.RestoreSucceeded,
    user,
  };
}

export function restoreFailed(): RestoreFailedAction {
  return {
    type: SessionActionTypes.RestoreFailed,
  };
}
