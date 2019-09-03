import { CustomAction, DataAction } from "./root";
import { Point } from "../viewModel/board/model";
import ThemeFactory from "../themes/themeFactory";
import ThemeService from "../themes/themeService";

export interface State {
    boardZoomLevel : number,
    boardScrollPercent : Point,
    boardContainerSize : Point,
    canvasMargin : number,
    canvasContentPadding : number,
    theme : Theme
}

export const defaultState : State = {
    boardZoomLevel: 0,
    boardScrollPercent: { x: 0.5, y: 0.5 },
    boardContainerSize: { x: 1000, y: 1000 },
    canvasContentPadding: 5,
    canvasMargin: 5,
    theme: ThemeFactory.default
};

export enum ActionTypes {
    BoardZoom = "BOARD_ZOOM",
    BoardScroll = "BOARD_SCROLL",
    BoardAreaResize = "BOARD_AREA_RESIZE",
    ChangeTheme = "CHANGE_THEME"
}

export class Actions {
    public static boardScroll(percent : Point) {
        return {
            type: ActionTypes.BoardScroll,
            data: percent
        };
    }

    public static boardZoom(level : number) {
        return {
            type: ActionTypes.BoardZoom,
            data: level
        };
    }

    public static boardAreaResize(size : Point) {
        return {
            type: ActionTypes.BoardAreaResize,
            data: size
        };
    }

    public static changeTheme(themeName : string) {
        return {
            type: ActionTypes.ChangeTheme,
            data: themeName
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.BoardAreaResize: {
            const da = <DataAction<Point>>action;
            const newState = {...state};
            newState.boardContainerSize = da.data;
            return newState;
        }
        case ActionTypes.ChangeTheme: {
            const da = <DataAction<string>>action;
            const newState = {...state};
            const theme = ThemeFactory.getThemes().get(da.data);
            newState.theme = theme;
            ThemeService.applyToCss(theme);
            return newState;
        }
        //BoardScroll and BoardZoom must be handled at a higher level because they update the boardview
        default:
            return state;
    }
}