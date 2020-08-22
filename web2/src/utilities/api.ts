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
  Middleware,
} from '../api-client';
import { RootState } from '../redux/root';
import { store } from '../redux';
import { Notification, NotificationLevel } from '../model/notifications';
import { addNotification } from '../redux/notifications/actionFactory';
import { Problem } from '../model/errorHandling';

function getApiUrl(state: RootState): string {
  return state.config.environment.apiUrl;
}

function getNotificationsMiddleware(): Middleware {
  return {
    post: async (context) => {
      const r = context.response;
      if (r.status > 399) {
        const text = await r.text();
        const problem = JSON.parse(text) as Problem;
        const notification: Notification = {
          id: '',
          message: problem.Title,
          time: new Date(),
          level: NotificationLevel.Error,
        };
        const action = addNotification(notification);
        store.dispatch(action);
      }
    },
  };
}

function getConfigParams(apiUrl: string): ConfigurationParameters {
  return {
    basePath: apiUrl,
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    middleware: [
      getNotificationsMiddleware(),
    ],
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
