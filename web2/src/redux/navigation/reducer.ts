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

    case NavigationActionTypes.OpenDrawer:
      return {
        ...state,
        isDrawerOpen: true,
      };

    case NavigationActionTypes.CloseDrawer:
      return {
        ...state,
        isDrawerOpen: false,
      };

    default:
      return state;
  }
}
