import ThemeService from "./themes/themeService";
import ApiClient from "./api/client";
import BoardViewService from "./boardRendering/boardViewService";

export default class Kernel {
    public static theme : ThemeService;
    public static api : ApiClient;
    public static boardViews : BoardViewService;
}