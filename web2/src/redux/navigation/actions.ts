import { NavigationActionTypes } from './actionTypes';

export type NavigateToAction = {
  type: typeof NavigationActionTypes.NavigateTo,
  value: string
};

export type NavigationAction =
  NavigateToAction;
