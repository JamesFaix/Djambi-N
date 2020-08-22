import { UserConfig, EnvironmentConfig } from '../model/configuration';
import { store } from '../redux';
import { defaultConfigState } from '../redux/config/state';
import { userConfigLoaded, environmentConfigLoaded, userConfigChanged } from '../redux/config/actionFactory';

const localStorageKey = 'Djambi_UserConfig';

// Using async functions so that user config can be persisted on the server
// later without changing the contract.

async function loadUserConfig(): Promise<void> {
  const json = localStorage.getItem(localStorageKey);

  const config = json
    ? JSON.parse(json) as UserConfig
    : defaultConfigState.user;

  const action = userConfigLoaded(config);
  store.dispatch(action);
}

async function loadEnvConfig(): Promise<void> {
  const response = await fetch('/env.json');
  const config = await response.json() as EnvironmentConfig;
  const action = environmentConfigLoaded(config);
  store.dispatch(action);
}

export async function loadConfig(): Promise<void> {
  await loadUserConfig();
  await loadEnvConfig();
}

export async function setUserConfig(config: UserConfig): Promise<void> {
  const json = JSON.stringify(config);
  localStorage.setItem(localStorageKey, json);
  const action = userConfigChanged(config);
  store.dispatch(action);
}
