import { UserConfig, EnvironmentConfig } from '../../model/configuration';

export type ConfigState = {
  user: UserConfig,
  environment: EnvironmentConfig
};

export const defaultConfigState: ConfigState = {
  user: {
    favoriteWord: 'scrupulous',
    logRedux: false,
  },
  environment: {
    apiUrl: '',
  },
};
