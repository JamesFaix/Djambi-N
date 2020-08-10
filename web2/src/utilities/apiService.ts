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

const configParams: ConfigurationParameters = {
  basePath: '',
  middleware: [],
  headers: {

  },
  credentials: 'include',
};

const config = new Configuration(configParams);

interface IApiService {
  boards: BoardApi,
  events: EventApi,
  games: GameApi,
  notifications: NotificationApi,
  players: PlayerApi,
  search: SearchApi,
  sessions: SessionApi,
  turns: TurnApi,
  users: UserApi
}

class ApiService implements IApiService {
  constructor(c: Configuration) {
    this.boards = new BoardApi(c);
    this.events = new EventApi(c);
    this.games = new GameApi(c);
    this.notifications = new NotificationApi(c);
    this.players = new PlayerApi(c);
    this.search = new SearchApi(c);
    this.sessions = new SessionApi(c);
    this.snapshots = new SnapshotApi(c);
    this.turns = new TurnApi(c);
    this.users = new UserApi(c);
  }

  boards: BoardApi;

  events: EventApi;

  games: GameApi;

  notifications: NotificationApi;

  players: PlayerApi;

  search: SearchApi;

  sessions: SessionApi;

  snapshots: SnapshotApi;

  turns: TurnApi;

  users: UserApi;
}

export const apiService = new ApiService(config);
