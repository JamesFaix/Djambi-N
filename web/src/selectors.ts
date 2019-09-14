import { Game, User, Board } from "./api/model";
import { useSelector } from "react-redux";
import { State as AppState } from "./store/root";
import { Theme } from "./themes/model";

export default class Selectors {
    public static game() : Game {
        return useSelector((state : AppState) => state.activeGame.game);
    }

    public static user() : User {
        return useSelector((state : AppState) => state.session.user);
    }

    public static theme() : Theme {
        return useSelector((state : AppState) => state.display.theme);
    }

    public static board(regionCount : number) : Board {
        return useSelector((state : AppState) => state.boards.boards.get(regionCount));
    }
}