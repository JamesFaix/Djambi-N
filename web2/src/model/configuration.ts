export type UserConfig = {
  favoriteWord: string;
  logRedux: boolean;
};

export type EnvironmentConfig = {
  apiUrl: string;
};

export type Config = {
  environment: EnvironmentConfig;
  user: UserConfig;
};
