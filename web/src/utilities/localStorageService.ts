import { DebugSettings } from "../debug";

const keyPrefix = "Djambi_";

export default class LocalStorageService {
    static get themeName() : string {
        return localStorage.getItem(`${keyPrefix}themeName`);
    }
    static set themeName(value : string) {
        localStorage.setItem(`${keyPrefix}themeName`, value);
    }

    static get debugSettings() : DebugSettings {
        const json = localStorage.getItem(`${keyPrefix}debugSettings`);
        const obj = JSON.parse(json);
        return obj;
    }
    static set debugSettings(value : DebugSettings) {
        const json = JSON.stringify(value);
        localStorage.setItem(`${keyPrefix}debugSettings`, json);
    }
}