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
import { ActiveGameState, defaultActiveGameState } from './activeGame/state';
import { ActiveGameActionTypes } from './activeGame/actionTypes';
import { ActiveGameAction } from './activeGame/actionts';
import { activeGameReducer } from './activeGame/reducer';

export type RootState = {
  activeGame: ActiveGameState,
  apiClient: ApiClientState,
  config: ConfigState,
  navigation: NavigationState,
  session: SessionState
};

export type RootActionTypes =
  ActiveGameActionTypes |
  ApiClientActionTypes |
  ConfigActionTypes |
  NavigationActionTypes |
  SessionActionTypes;

export type RootAction =
  ActiveGameAction |
  ApiClientAction |
  ConfigAction |
  NavigationAction |
  SessionAction;

export const defaultRootState: RootState = {
  activeGame: defaultActiveGameState,
  apiClient: defaultApiClientState,
  config: defaultConfigState,
  navigation: defaultNavigationState,
  session: defaultSessionState,
};

export const rootReducer = combineReducers({
  activeGame: activeGameReducer,
  apiClient: apiClientReducer,
  config: configReducer,
  navigation: navigationReducer,
  session: sessionReducer,
});
