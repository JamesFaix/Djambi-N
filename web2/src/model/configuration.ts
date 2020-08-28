export type UserConfig = {
  logRedux: boolean;
  notificationDisplaySeconds: number;
};

export type EnvironmentConfig = {
  apiUrl: string;
};

export type Config = {
  environment: EnvironmentConfig;
  user: UserConfig;
};
