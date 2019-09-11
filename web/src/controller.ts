import { AppStore, CustomAction, State } from "./store/root";
import { History } from "history";
import { DebugSettings, defaultDebugSettings } from "./debug";
import LocalStorageService from "./utilities/localStorageService";
import * as StoreSettings from './store/settings';
import * as StoreGamesQuery from './store/gamesQuery';
import * as StoreActiveGame from './store/activeGame';
import * as StoreSession from './store/session';
import * as StoreBoards from './store/boards';
import * as StoreCreateGameForm from './store/createGameForm';
import * as Api from './api/client';
import {
    GamesQuery,
    CreateSnapshotRequest,
    LoginRequest,
    CreateUserRequest,
    User,
    EventsQuery,
    GameParameters,
    CreatePlayerRequest,
    StateAndEventResponse,
    GameStatus,
    Game,
    ResultsDirection,
    Event,
    Board,
    PlayerStatus
} from "./api/model";
import { SseClientManager } from "./utilities/serverSentEvents";
import ThemeService from "./themes/themeService";
import Routes from "./routes";
import * as ModelFactory from "./api/modelFactory";

//Encapsulates dispatching Redux actions and other side effects
export default class Controller {
    private static store : AppStore;
    private static history : History<any>;

    public static init(
        store : AppStore,
        history : History<any>
    ) : void {
        this.store = store;
        this.history = history;
    }

    private static dispatch(action : CustomAction) {
        Controller.store.dispatch(action);
    }

    private static get state() : State {
        return Controller.store.getState();
    }

    public static navigateTo(route : string) : void {
        Controller.history.push(route);
        if (Controller.state.settings.debug.logRedux) {
            console.log(`Navigate to ${route}`);
        }
    }

    public static Settings = class {
        public static saveAndApply(settings : DebugSettings) : void {
            LocalStorageService.debugSettings = settings;
            Controller.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }

        public static loadAndApply() : void {
            let settings = LocalStorageService.debugSettings;
            if (!settings) { settings = defaultDebugSettings; }
            Controller.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }
    }

    public static async queryGames(query: GamesQuery) : Promise<void> {
        const games = await Api.getGames(query);
        Controller.dispatch(StoreGamesQuery.Actions.queryGames(games));
    }

    public static Snapshots = class {
        public static async get(gameId: number) : Promise<void> {
            const snapshots = await Api.getSnapshotsForGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.loadSnapshots(snapshots))
        }

        public static async save(gameId : number, request : CreateSnapshotRequest) : Promise<void> {
            const snapshot = await Api.createSnapshot(gameId, request);
            Controller.dispatch(StoreActiveGame.Actions.snapshotSaved(snapshot));
        }

        public static async load(gameId : number, snapshotId : number) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            return Controller.Game.loadGameFull(gameId);
        }

        public static async delete(gameId : number, snapshotId : number) : Promise<void>{
            await Api.loadSnapshot(gameId, snapshotId);
            Controller.dispatch(StoreActiveGame.Actions.snapshotDeleted(snapshotId));
        }
    }

    public static Session = class {
        public static async login(request : LoginRequest) : Promise<void> {
            const session = await Api.login(request);
            Controller.dispatch(StoreSession.Actions.login(session.user));
            Controller.navigateTo(Routes.dashboard);
            return Controller.Session.finishLoginSetup(session.user);
        }

        public static async logout() : Promise<void> {
            await Api.logout();
            Controller.dispatch(StoreSession.Actions.logout());
            Controller.navigateTo(Routes.login);
            SseClientManager.disconnect();
        }

        public static async signup(request : CreateUserRequest) : Promise<void> {
            const user = await Api.createUser(request);
            Controller.dispatch(StoreSession.Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return Controller.Session.login(loginRequest);
        }

        public static async redirectToDashboardIfLoggedIn() : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                Controller.dispatch(StoreSession.Actions.restoreSession(user));
                Controller.navigateTo(Routes.dashboard);
                return Controller.Session.finishLoginSetup(user);
            }
            catch(ex) {
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
            }
        }

        public static async redirectToLoginIfNotLoggedIn() : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                Controller.dispatch(StoreSession.Actions.restoreSession(user));
                return Controller.Session.finishLoginSetup(user);
            }
            catch (ex) {
                console.log(ex);
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
                Controller.navigateTo(Routes.login);
            }
        }

        public static async redirectToLoginOrDashboard() : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                Controller.dispatch(StoreSession.Actions.restoreSession(user));
                Controller.navigateTo(Routes.dashboard);
                return Controller.Session.finishLoginSetup(user);
            }
            catch (ex) {
                console.log(ex);
                let [status, message] = ex;
                if (status === 401) {
                    Controller.navigateTo(Routes.login);
                }
                else {
                    throw ex;
                }
            }
        }

        private static finishLoginSetup(user : User) : Promise<void> {
            SseClientManager.connect();
            ThemeService.loadSavedTheme(Controller.dispatch);
            Controller.Settings.loadAndApply();
            return Controller.Session.queryGamesForUser(user);
        }

        private static queryGamesForUser(user: User) : Promise<void> {
            const query = ModelFactory.emptyGamesQuery();
            query.playerUserName = user.name;
            Controller.dispatch(StoreGamesQuery.Actions.updateGamesQuery(query));
            return Controller.queryGames(query);
        }
    }

    public static Game = class {
        private static async loadGameInner(gameId : number) : Promise<Game> {
            const game = await Api.getGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.loadGame(game));
            return game;
        }

        private static async loadHistoryInner(gameId : number) : Promise<Event[]> {
            const query : EventsQuery = {
                maxResults : null,
                direction : ResultsDirection.Descending,
                thresholdTime : null,
                thresholdEventId : null
            };

            const history = await Api.getEvents(gameId, query);
            Controller.dispatch(StoreActiveGame.Actions.loadGameHistory(history));
            return history;
        }

        public static async loadAllBoards() : Promise<void> {
            for (let i=3; i<=8; i++){
                await Controller.Game.loadBoardInner(i);
            }
        }

        private static async loadBoardInner(regionCount : number) : Promise<Board> {
            const board = await Api.getBoard(regionCount);
            Controller.dispatch(StoreBoards.Actions.loadBoard(board));
            return board;
        }

        public static async loadGame(gameId: number) : Promise<void> {
            await Controller.Game.loadGameInner(gameId);
        }

        public static async loadGameFull(gameId : number) : Promise<void> {
            const game = await Controller.Game.loadGameInner(gameId);
            await Controller.Game.loadHistoryInner(gameId);
            await Controller.Game.loadBoardInner(game.parameters.regionCount);
        }

        public static async createGame(formData : GameParameters) : Promise<void> {
            const game = await Api.createGame(formData);
            Controller.dispatch(StoreActiveGame.Actions.createGame(game));
            Controller.navigateTo(Routes.lobby(game.id));
        }

        public static async addPlayer(gameId: number, request : CreatePlayerRequest) : Promise<void> {
            const resp = await Api.addPlayer(gameId, request);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async removePlayer(gameId: number, playerId: number) : Promise<void> {
            const resp = await Api.removePlayer(gameId, playerId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async startGame(gameId: number) : Promise<void> {
            const resp = await Api.startGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
            Controller.navigateTo(Routes.play(gameId));
        }

        public static async selectCell(gameId: number, cellId : number) : Promise<void> {
            const resp = await Api.selectCell(gameId, cellId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async endTurn(gameId: number) : Promise<void> {
            const resp = await Api.commitTurn(gameId);
            Controller.Game.updateInProgressGame(resp);
        }

        public static async resetTurn(gameId: number) : Promise<void> {
            const resp = await Api.resetTurn(gameId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async changePlayerStatus(gameId: number, playerId: number, status: PlayerStatus) : Promise<void> {
            const resp = await Api.updatePlayerStatus(gameId, playerId, status);
            Controller.Game.updateInProgressGame(resp);
        }

        public static updateInProgressGame(response : StateAndEventResponse) : void {
            Controller.dispatch(StoreActiveGame.Actions.updateGame(response));
            const g = response.game;
            if (g.status === GameStatus.Over) {
                Controller.navigateTo(Routes.gameOver(g.id));
            }
        }
    }

    public static Forms = class {
        public static updateCreateGameForm(formData : GameParameters) : void {
            Controller.dispatch(StoreCreateGameForm.Actions.updateCreateGameForm(formData))
        }

        public static updateGamesQuery(formData : GamesQuery) : void {
            Controller.dispatch(StoreGamesQuery.Actions.updateGamesQuery(formData))
        }
    }
}