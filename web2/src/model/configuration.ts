export type UserConfig = {
  favoriteWord: string;
};

export type EnvironmentConfig = {
  apiUrl: string;
};

export type Config = {
  environment: EnvironmentConfig;
  user: UserConfig;
};
