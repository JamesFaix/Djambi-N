import { ConfigActionTypes } from './actionTypes';
import { UserConfig, EnvironmentConfig } from '../../model/configuration';

export type UpdateUserConfigAction = {
  type: typeof ConfigActionTypes.UserConfigChanged,
  value: UserConfig
};

export type UserConfigLoadedAction = {
  type: typeof ConfigActionTypes.UserConfigLoaded,
  value: UserConfig
};

export type EnvironmentConfigLoadedAction = {
  type: typeof ConfigActionTypes.EnvironmentConfigLoaded,
  value: EnvironmentConfig
};

export type ConfigAction =
  UpdateUserConfigAction |
  UserConfigLoadedAction |
  EnvironmentConfigLoadedAction;
