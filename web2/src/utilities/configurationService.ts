import { Config, UserConfig, EnvironmentConfig } from '../model/configuration';

// #region Local storage

const localStorageKeyPrefix = 'Apex_';

// Using async functions so that user config can be persisted on the server
// later without changing the contract.

export async function getUserConfig(): Promise<UserConfig> {
  return {
    favoriteWord: localStorage.getItem(`${localStorageKeyPrefix}favoriteWord`) || '',
  };
}

export async function setUserConfig(config: UserConfig): Promise<void> {
  localStorage.setItem(`${localStorageKeyPrefix}favoriteWord`, config.favoriteWord || '');
}

// #endregion

// #region Config file

let envConfig: EnvironmentConfig | null = null;

async function loadEnvConfig(): Promise<EnvironmentConfig> {
  const response = await fetch('/environmentConfig.json');
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
