import { combineReducers } from 'redux';
import { configReducer } from './config/reducer';
import { apiClientReducer } from './apiClient/reducer';
import { ApiClientState, defaultApiClientState } from './apiClient/state';
import { ConfigState, defaultConfigState } from './config/state';
import { ApiClientAction } from './apiClient/actions';
import { ConfigAction } from './config/actions';
import { ApiClientActionTypes } from './apiClient/actionTypes';
import { ConfigActionTypes } from './config/actionTypes';
import { SessionState, defaultSessionState } from './session/state';
import { SessionActionTypes } from './session/actionTypes';
import { SessionAction } from './session/actions';
import { sessionReducer } from './session/reducer';
import { NavigationState, defaultNavigationState } from './navigation/state';
import { NavigationActionTypes } from './navigation/actionTypes';
import { NavigationAction } from './navigation/actions';
import { navigationReducer } from './navigation/reducer';

export type RootState = {
  apiClient: ApiClientState,
  config: ConfigState,
  navigation: NavigationState,
  session: SessionState
};

export type RootActionTypes =
  ApiClientActionTypes |
  ConfigActionTypes |
  NavigationActionTypes |
  SessionActionTypes;

export type RootAction =
  ApiClientAction |
  ConfigAction |
  NavigationAction |
  SessionAction;

export const defaultRootState: RootState = {
  apiClient: defaultApiClientState,
  config: defaultConfigState,
  navigation: defaultNavigationState,
  session: defaultSessionState,
};

export const rootReducer = combineReducers({
  apiClient: apiClientReducer,
  config: configReducer,
  navigation: navigationReducer,
  session: sessionReducer,
});
