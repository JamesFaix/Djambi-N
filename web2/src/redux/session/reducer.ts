import { SessionState, defaultSessionState } from './state';
import { SessionAction } from './actions';
import { SessionActionTypes } from './actionTypes';

export function sessionReducer(
  state: SessionState = defaultSessionState,
  action: SessionAction,
): SessionState {
  switch (action.type) {
    case SessionActionTypes.LoggedIn:
      return {
        ...state,
        user: action.user,
      };

    case SessionActionTypes.LoggedOut:
      return {
        ...state,
        user: null,
      };

    case SessionActionTypes.Restored:
      return {
        ...state,
        user: action.user,
      };

    default:
      return state;
  }
}
