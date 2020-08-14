import { NavigationActionTypes } from './actionTypes';

export type NavigateToAction = {
  type: typeof NavigationActionTypes.NavigateTo,
  value: string
};

export type OpenNavigationDrawerAction = {
  type: typeof NavigationActionTypes.OpenDrawer,
};

export type CloseNavigationDrawerAction = {
  type: typeof NavigationActionTypes.CloseDrawer,
};

export type NavigationAction =
  NavigateToAction |
  OpenNavigationDrawerAction |
  CloseNavigationDrawerAction;
