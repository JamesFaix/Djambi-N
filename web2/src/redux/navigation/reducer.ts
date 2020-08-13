import { defaultNavigationState, NavigationState } from './state';
import { NavigationAction } from './actions';
import { NavigationActionTypes } from './actionTypes';

export function navigationReducer(
  state: NavigationState = defaultNavigationState,
  action: NavigationAction,
): NavigationState {
  switch (action.type) {
    case NavigationActionTypes.NavigateTo:
      return {
        ...state,
        path: action.value,
      };
    default:
      return state;
  }
}
