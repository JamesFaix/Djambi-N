import ThemeService from "./themes/themeService";
import ApiClient from "./api/client";
import BoardViewService from "./boardRendering/boardViewService";

export class Kernel {
    private static _theme : ThemeService;
    public static get theme() { return this._theme; }

    private static _api : ApiClient;
    public static get api() { return this._api; }

    private static _boardViews : BoardViewService;
    public static get boardViews() { return this._boardViews; }

    public static initialize() {
        this._theme = new ThemeService();
        this._api = new ApiClient();
        this._boardViews = new BoardViewService(this._api);
    }
}