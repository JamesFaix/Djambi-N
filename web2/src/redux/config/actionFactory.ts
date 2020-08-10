import { UserConfig, EnvironmentConfig } from '../../model/configuration';
import { UpdateUserConfigAction, UserConfigLoadedAction, EnvironmentConfigLoadedAction } from './actions';
import { ConfigActionTypes } from './actionTypes';

export function userConfigChanged(config: UserConfig): UpdateUserConfigAction {
  return {
    type: ConfigActionTypes.UserConfigChanged,
    value: config,
  };
}

export function userConfigLoaded(config: UserConfig): UserConfigLoadedAction {
  return {
    type: ConfigActionTypes.UserConfigLoaded,
    value: config,
  };
}

export function environmentConfigLoaded(config: EnvironmentConfig): EnvironmentConfigLoadedAction {
  return {
    type: ConfigActionTypes.EnvironmentConfigLoaded,
    value: config,
  };
}
