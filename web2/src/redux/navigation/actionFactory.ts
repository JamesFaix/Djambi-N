import { NavigateToAction, OpenNavigationDrawerAction, CloseNavigationDrawerAction } from './actions';
import { NavigationActionTypes } from './actionTypes';

export function navigateTo(path: string): NavigateToAction {
  return {
    type: NavigationActionTypes.NavigateTo,
    value: path,
  };
}

export function openDrawer(): OpenNavigationDrawerAction {
  return {
    type: NavigationActionTypes.OpenDrawer,
  };
}

export function closeDrawer(): CloseNavigationDrawerAction {
  return {
    type: NavigationActionTypes.CloseDrawer,
  };
}
