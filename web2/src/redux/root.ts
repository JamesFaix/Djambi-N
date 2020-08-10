import { combineReducers } from 'redux';
import { configReducer } from './config/reducer';
import { apiClientReducer } from './apiClient/reducer';
import { ApiClientState, defaultApiClientState } from './apiClient/state';
import { ConfigState, defaultConfigState } from './config/state';
import { ApiClientAction } from './apiClient/actions';
import { ConfigAction } from './config/actions';
import { ApiClientActionTypes } from './apiClient/actionTypes';
import { ConfigActionTypes } from './config/actionTypes';

export type RootState = {
  apiClient: ApiClientState,
  config: ConfigState
};

export type RootActionTypes =
  ApiClientActionTypes |
  ConfigActionTypes;

export type RootAction =
  ApiClientAction |
  ConfigAction;

export const defaultRootState: RootState = {
  apiClient: defaultApiClientState,
  config: defaultConfigState,
};

export const rootReducer = combineReducers({
  apiClient: apiClientReducer,
  config: configReducer,
});
