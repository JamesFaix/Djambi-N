import { NavigateToAction } from './actions';
import { NavigationActionTypes } from './actionTypes';

export function navigateTo(path: string): NavigateToAction {
  return {
    type: NavigationActionTypes.NavigateTo,
    value: path,
  };
}
