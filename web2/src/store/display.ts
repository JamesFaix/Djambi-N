import { CustomAction, DataAction } from "./root";
import { Point } from "../viewModel/board/model";

export interface State {
    boardZoomLevel : number,
    boardScrollPercent : Point,
    boardContainerSize : Point,
    canvasMargin : number,
    canvasContentPadding : number,
}

export const defaultState : State = {
    boardZoomLevel: 0,
    boardScrollPercent: { x: 0.5, y: 0.5 },
    boardContainerSize: { x: 1000, y: 1000 },
    canvasContentPadding: 5,
    canvasMargin: 5,
};

export enum ActionTypes {
    BoardZoom = "BOARD_ZOOM",
    BoardScroll = "BOARD_SCROLL",
    BoardAreaResize = "BOARD_AREA_RESIZE"
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
        //BoardScroll and BoardZoom must be handled at a higher level because they update the boardview
        default:
            return state;
    }
}