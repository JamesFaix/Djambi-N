import { UserDto } from '../../api-client';
import { LoggedInAction, LoggedOutAction } from './actions';
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
