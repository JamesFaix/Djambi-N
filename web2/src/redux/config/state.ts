import { UserConfig, EnvironmentConfig } from '../../model/configuration';

export type ConfigState = {
  user: UserConfig,
  environment: EnvironmentConfig
};

export const defaultConfigState: ConfigState = {
  user: {
    logRedux: false,
    showBoardTooltips: false,
    showCellAndPieceIds: false,
    notificationDisplaySeconds: 5,
  },
  environment: {
    apiUrl: '',
  },
};
