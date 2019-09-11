import { AppStore } from "../store/root";
import { History } from "history";
import { DebugSettings, defaultDebugSettings } from "../debug";
import LocalStorageService from "../utilities/localStorageService";
import * as StoreSettings from '../store/settings';
import * as StoreGamesQuery from '../store/gamesQuery';
import * as StoreActiveGame from '../store/activeGame';
import * as Api from '../api/client';
import { GamesQuery, CreateSnapshotRequest } from "../api/model";
import GameStoreFlows from "./game";

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


}