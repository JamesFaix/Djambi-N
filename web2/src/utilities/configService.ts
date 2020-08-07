import { Config, UserConfig, EnvironmentConfig } from '../model/configuration';

// #region User config

const key = 'Apex_UserConfig';

const defaultUserConfig: UserConfig = {
  favoriteWord: 'scrupulous',
};

// Using async functions so that user config can be persisted on the server
// later without changing the contract.

export async function getUserConfig(): Promise<UserConfig> {
  const json = localStorage.getItem(key);

  const config = json
    ? JSON.parse(json) as UserConfig
    : defaultUserConfig;

  return config;
}

export async function setUserConfig(config: UserConfig): Promise<void> {
  const json = JSON.stringify(config);
  localStorage.setItem(key, json);
}

// #endregion

// #region Environment config

let envConfig: EnvironmentConfig | null = null;

async function loadEnvConfig(): Promise<EnvironmentConfig> {
  const response = await fetch('/env.json');
  const config = await response.json();
  return config as EnvironmentConfig;
}

export async function getEnvironmentConfig(): Promise<EnvironmentConfig> {
  if (envConfig === null) {
    envConfig = await loadEnvConfig();
  }
  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  return envConfig!;
}

// #endregion

export async function getConfig(): Promise<Config> {
  return {
    environment: await getEnvironmentConfig(),
    user: await getUserConfig(),
  };
}
