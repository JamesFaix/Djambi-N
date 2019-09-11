import { AppStore } from "../store/root";
import { History } from "history";
import { DebugSettings, defaultDebugSettings } from "../debug";
import LocalStorageService from "../utilities/localStorageService";
import * as StoreSettings from '../store/settings';
import * as StoreGamesQuery from '../store/gamesQuery';
import * as StoreActiveGame from '../store/activeGame';
import * as StoreSession from '../store/session';
import * as Api from '../api/client';
import { GamesQuery, CreateSnapshotRequest, LoginRequest, CreateUserRequest, User } from "../api/model";
import GameStoreFlows from "./game";
import { SseClientManager } from "../utilities/serverSentEvents";
import ThemeService from "../themes/themeService";
import Routes from "../routes";
import * as ModelFactory from "../api/modelFactory";

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

    public static navigateTo(route : string) : void {
        Controller.history.push(route);
    }

    public static Settings = class {
        public static saveAndApply(settings : DebugSettings) : void {
            LocalStorageService.debugSettings = settings;
            Controller.store.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }

        public static loadAndApply() : void {
            let settings = LocalStorageService.debugSettings;
            if (!settings) { settings = defaultDebugSettings; }
            Controller.store.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }
    }

    public static async queryGames(query: GamesQuery) : Promise<void> {
        const games = await Api.getGames(query);
        Controller.store.dispatch(StoreGamesQuery.Actions.queryGames(games));
    }

    public static Snapshots = class {
        public static async get(gameId: number) : Promise<void> {
            const snapshots = await Api.getSnapshotsForGame(gameId);
            Controller.store.dispatch(StoreActiveGame.Actions.loadSnapshots(snapshots))
        }

        public static async save(gameId : number, request : CreateSnapshotRequest) : Promise<void> {
            const snapshot = await Api.createSnapshot(gameId, request);
            Controller.store.dispatch(StoreActiveGame.Actions.snapshotSaved(snapshot));
        }

        public static async load(gameId : number, snapshotId : number) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            return GameStoreFlows.loadGameFull(gameId)(Controller.store.dispatch);
        }

        public static async delete(gameId : number, snapshotId : number) : Promise<void>{
            await Api.loadSnapshot(gameId, snapshotId);
            Controller.store.dispatch(StoreActiveGame.Actions.snapshotDeleted(snapshotId));
        }
    }

    public static Session = class {
        public static async login(request : LoginRequest) : Promise<void> {
            const session = await Api.login(request);
            Controller.store.dispatch(StoreSession.Actions.login(session.user));
            Controller.navigateTo(Routes.dashboard);
            return Controller.Session.finishLoginSetup(session.user);
        }

        public static async logout() : Promise<void> {
            await Api.logout();
            Controller.store.dispatch(StoreSession.Actions.logout());
            Controller.navigateTo(Routes.login);
            SseClientManager.disconnect();
        }

        public static async signup(request : CreateUserRequest) : Promise<void> {
            const user = await Api.createUser(request);
            Controller.store.dispatch(StoreSession.Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return Controller.Session.login(loginRequest);
        }

        public static async redirectToDashboardIfLoggedIn() : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                Controller.store.dispatch(StoreSession.Actions.restoreSession(user));
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
                Controller.store.dispatch(StoreSession.Actions.restoreSession(user));
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
                Controller.store.dispatch(StoreSession.Actions.restoreSession(user));
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
            ThemeService.loadSavedTheme(Controller.store.dispatch);
            Controller.Settings.loadAndApply();
            return Controller.Session.queryGamesForUser(user);
        }

        private static queryGamesForUser(user: User) : Promise<void> {
            const query = ModelFactory.emptyGamesQuery();
            query.playerUserName = user.name;
            Controller.store.dispatch(StoreGamesQuery.Actions.updateGamesQuery(query));
            return Controller.queryGames(query);
        }
    }

}