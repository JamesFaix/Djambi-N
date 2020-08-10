import { ConfigState, defaultConfigState } from './state';
import { ConfigAction } from './actions';
import { ConfigActionTypes } from './actionTypes';

export function configReducer(
  state: ConfigState = defaultConfigState,
  action: ConfigAction,
): ConfigState {
  switch (action.type) {
    case ConfigActionTypes.UserConfigChanged:
      return {
        ...state,
        user: action.value,
      };
    case ConfigActionTypes.UserConfigLoaded:
      return {
        ...state,
        user: action.value,
      };
    case ConfigActionTypes.EnvironmentConfigLoaded:
      return {
        ...state,
        environment: action.value,
      };
    default:
      return state;
  }
}
