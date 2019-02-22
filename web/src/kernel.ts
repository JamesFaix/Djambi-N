import ThemeService from "./themes/themeService";
import ApiClient from "./api/client";
import { Styles, Classes } from "./styles";
import DateService from "./dateService";
import Routes from "./routes";
import CopyService from "./copyService";
import BoardService from "./boardService";

export class Kernel {

    private static _api : ApiClient;
    public static get api() { return this._api; }

    private static _boards : BoardService;
    public static get boards() { return this._boards; }

    private static _classes : Classes;
    public static get classes() { return this._classes; }

    private static _copy : CopyService;
    public static get copy() { return this._copy; }

    private static _dates : DateService;
    public static get dates() { return this._dates; }

    private static _routes : Routes;
    public static get routes() { return this._routes; }

    private static _styles : Styles;
    public static get styles() { return this._styles; }

    private static _theme : ThemeService;
    public static get theme() { return this._theme; }

    public static initialize() {
        this._api = new ApiClient();
        this._classes = new Classes();
        this._dates = new DateService();
        this._routes = new Routes();
        this._styles = new Styles();
        this._theme = new ThemeService();

        this._boards = new BoardService(this._api);
        this._copy = new CopyService(this._boards, this._theme);
    }
}