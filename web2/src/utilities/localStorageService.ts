const keyPrefix = "Djambi_";

export default class LocalStorageService {
    static get themeName() : string {
        return localStorage.getItem(`${keyPrefix}themeName`);
    }
    static set themeName(value : string) {
        localStorage.setItem(`${keyPrefix}themeName`, value);
    }
}