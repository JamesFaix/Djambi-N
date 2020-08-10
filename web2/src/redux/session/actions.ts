import { SessionActionTypes } from './actionTypes';
import { UserDto } from '../../api-client';

export type LoggedInAction = {
  type: typeof SessionActionTypes.LoggedIn,
  user: UserDto
};

export type LoggedOutAction = {
  type: typeof SessionActionTypes.LoggedOut,
};

export type SessionAction =
  LoggedInAction |
  LoggedOutAction;
