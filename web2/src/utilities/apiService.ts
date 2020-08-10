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
import { ApexStore, store } from '../redux';

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

function getConfig(store: ApexStore): Configuration {
  const state = store.getState();
  const apiUrl = getApiUrl(state);
  const params = getConfigParams(apiUrl);
  const config = new Configuration(params);
  return config;
}

class ApiService {
  constructor(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private store: ApexStore,
  ) {
  }

  get boards(): BoardApi {
    const config = getConfig(this.store);
    return new BoardApi(config);
  }

  get events(): EventApi {
    const config = getConfig(this.store);
    return new EventApi(config);
  }

  get games(): GameApi {
    const config = getConfig(this.store);
    return new GameApi(config);
  }

  get notifications(): NotificationApi {
    const config = getConfig(this.store);
    return new NotificationApi(config);
  }

  get players(): PlayerApi {
    const config = getConfig(this.store);
    return new PlayerApi(config);
  }

  get search(): SearchApi {
    const config = getConfig(this.store);
    return new SearchApi(config);
  }

  get sessions(): SessionApi {
    const config = getConfig(this.store);
    return new SessionApi(config);
  }

  get snapshots(): SnapshotApi {
    const config = getConfig(this.store);
    return new SnapshotApi(config);
  }

  get turns(): TurnApi {
    const config = getConfig(this.store);
    return new TurnApi(config);
  }

  get users(): UserApi {
    const config = getConfig(this.store);
    return new UserApi(config);
  }
}

export const apiService = new ApiService(store);
