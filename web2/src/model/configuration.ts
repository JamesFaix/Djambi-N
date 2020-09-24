export type UserConfig = {
  logRedux: boolean;
  notificationDisplaySeconds: number;
  showCellAndPieceIds: boolean;
};

export type EnvironmentConfig = {
  apiUrl: string;
};

export type Config = {
  environment: EnvironmentConfig;
  user: UserConfig;
};
