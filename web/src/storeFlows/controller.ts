import { AppStore } from "../store/root";
import { History } from "history";
import { DebugSettings, defaultDebugSettings } from "../debug";
import LocalStorageService from "../utilities/localStorageService";
import * as StoreSettings from '../store/settings';
import * as StoreGamesQuery from '../store/gamesQuery';
import * as Api from '../api/client';
import { GamesQuery } from "../api/model";

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
        this.history.push(route);
    }

    //#region Settings

    public static applySettings(settings : DebugSettings) : void {
        LocalStorageService.debugSettings = settings;
        this.store.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
    }

    public static loadSavedSettings() : void {
        let settings = LocalStorageService.debugSettings;
        if (!settings) { settings = defaultDebugSettings; }
        this.store.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
    }

    //#endregion

    public static async queryGames(query: GamesQuery) : Promise<void> {
        const games = await Api.getGames(query);
        this.store.dispatch(StoreGamesQuery.Actions.queryGames(games));
    }
}