import { Dispatch } from "redux";
import { GamesQuery } from "../api/model";
import * as Api from "../api/client";
import * as StoreGamesQuery from '../store/gamesQuery';
import { DebugSettings, defaultDebugSettings } from "../debug";
import LocalStorageService from "../utilities/localStorageService";
import * as StoreSettings from '../store/settings';

export default class MiscStoreFlows {
    public static queryGames(query: GamesQuery) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const games = await Api.getGames(query);
            dispatch(StoreGamesQuery.Actions.queryGames(games));
        };
    }

    public static applySettings(settings : DebugSettings, dispatch : Dispatch) : void {
        LocalStorageService.debugSettings = settings;
        dispatch(StoreSettings.Actions.applyDebugSettings(settings));
    }

    public static loadSavedSettings(dispatch : Dispatch) {
        let settings = LocalStorageService.debugSettings;
        if (!settings) { settings = defaultDebugSettings; }
        dispatch(StoreSettings.Actions.applyDebugSettings(settings));
    }
}