import { AppStore } from "../store/root";
import { History } from "history";

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
}