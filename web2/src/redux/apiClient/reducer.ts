import { ApiClientState, defaultApiClientState } from './state';
import { ApiClientAction } from './actions';
import { ApiClientActionTypes } from './actionTypes';

export function apiClientReducer(
  state: ApiClientState = defaultApiClientState,
  action: ApiClientAction,
): ApiClientState {
  switch (action.type) {
    case ApiClientActionTypes.RequestSent:
      return {
        ...state,
        pendingRequestIds: [...state.pendingRequestIds, action.requestId],
      };

    case ApiClientActionTypes.ResponseReceived:
      return {
        ...state,
        pendingRequestIds: state.pendingRequestIds.filter((x) => x !== action.requestId),
      };

    default:
      return state;
  }
}
