import {
  BoardApi,
  Configuration,
  ConfigurationParameters,
  EventApi,
  GameApi,
  NotificationApi,
  PlayerApi,
  SearchApi,
  SessionApi,
  SnapshotApi,
  TurnApi,
  UserApi,
} from '../api-client';
import { RootState } from '../redux/root';
import { store } from '../redux';

function getApiUrl(state: RootState): string {
  return state.config.environment.apiUrl;
}

function getConfigParams(apiUrl: string): ConfigurationParameters {
  return {
    basePath: apiUrl,
    middleware: [],
    headers: {

    },
    credentials: 'include',
  };
}

function getConfig(): Configuration {
  const state = store.getState();
  const apiUrl = getApiUrl(state);
  const params = getConfigParams(apiUrl);
  const config = new Configuration(params);
  return config;
}

export function boards(): BoardApi {
  const config = getConfig();
  return new BoardApi(config);
}

export function events(): EventApi {
  const config = getConfig();
  return new EventApi(config);
}

export function games(): GameApi {
  const config = getConfig();
  return new GameApi(config);
}

export function notifications(): NotificationApi {
  const config = getConfig();
  return new NotificationApi(config);
}

export function players(): PlayerApi {
  const config = getConfig();
  return new PlayerApi(config);
}

export function search(): SearchApi {
  const config = getConfig();
  return new SearchApi(config);
}

export function sessions(): SessionApi {
  const config = getConfig();
  return new SessionApi(config);
}

export function snapshots(): SnapshotApi {
  const config = getConfig();
  return new SnapshotApi(config);
}

export function turns(): TurnApi {
  const config = getConfig();
  return new TurnApi(config);
}

export function users(): UserApi {
  const config = getConfig();
  return new UserApi(config);
}
