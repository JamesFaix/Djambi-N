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

    case SessionActionTypes.AttemptingRestore:
      return {
        ...state,
        isRestorePending: true,
      };

    case SessionActionTypes.RestoreSucceeded:
      return {
        ...state,
        user: action.user,
        isRestorePending: false,
      };

    case SessionActionTypes.RestoreFailed:
      return {
        ...state,
        isRestorePending: false,
      };

    default:
      return state;
  }
}
