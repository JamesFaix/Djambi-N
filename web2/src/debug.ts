import { Dispatch } from "redux";
import LocalStorageService from "./utilities/localStorageService";
import * as StoreSettings from './store/settings';

export interface DebugSettings {
    showCellLabels : boolean,
    showCellAndPieceIds : boolean,
    logApi : boolean,
    logSse : boolean,
    logRedux : boolean
}

export const defaultDebugSettings : DebugSettings = {
    showCellLabels : false,
    showCellAndPieceIds : false,
    logApi : false,
    logSse : false,
    logRedux : false
}

export class DebugService {
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