import { combineReducers, createStore } from 'redux';
import { configReducer } from './config/reducer';
import { apiClientReducer } from './apiClient/reducer';
import { ApiClientState, defaultApiClientState } from './apiClient/state';
import { ConfigState, defaultConfigState } from './config/state';

export type RootState = {
  apiClient: ApiClientState,
  config: ConfigState
};

export const defaultRootState: RootState = {
  apiClient: defaultApiClientState,
  config: defaultConfigState,
};

export const rootReducer = combineReducers({
  apiClient: apiClientReducer,
  config: configReducer,
});

export const store = createStore(rootReducer, defaultRootState);
