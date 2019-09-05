import { CustomAction, DataAction } from "./root";
import { Point } from "../viewModel/board/model";
import ThemeFactory from "../themes/themeFactory";
import { PieceKind, Piece } from "../api/model";

interface Images {
    pieces : Map<PieceKind, HTMLImageElement>
}

export interface State {
    boardZoomLevel : number,
    boardScrollPercent : Point,
    boardContainerSize : Point,
    canvasMargin : number,
    canvasContentPadding : number,
    theme : Theme,
    images : Images
}

export const defaultState : State = {
    boardZoomLevel: 0,
    boardScrollPercent: { x: 0.5, y: 0.5 },
    boardContainerSize: { x: 1000, y: 1000 },
    canvasContentPadding: 5,
    canvasMargin: 5,
    theme: ThemeFactory.default,
    images: {
        pieces: new Map<PieceKind, HTMLImageElement>()
    }
};

export enum ActionTypes {
    BoardZoom = "BOARD_ZOOM",
    BoardScroll = "BOARD_SCROLL",
    BoardAreaResize = "BOARD_AREA_RESIZE",
    ChangeTheme = "CHANGE_THEME",
    PieceImageLoaded = "PIECE_IMAGE_LOADED"
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

    public static changeTheme(theme : Theme) {
        return {
            type: ActionTypes.ChangeTheme,
            data: theme
        };
    }

    public static pieceImageLoaded(pieceKind : PieceKind, image : HTMLImageElement) {
        return {
            type: ActionTypes.PieceImageLoaded,
            data: [pieceKind, image]
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.BoardAreaResize: {
            const da = <DataAction<Point>>action;
            return {
                ...state,
                boardContainerSize: da.data
            };
        }
        case ActionTypes.ChangeTheme: {
            const da = <DataAction<Theme>>action;
            return {
                ...state,
                theme: da.data,
                images: {
                    ...state.images,
                    pieces: new Map<PieceKind, HTMLImageElement>()
                }
            };
        }
        case ActionTypes.PieceImageLoaded: {
            const da = <DataAction<[PieceKind, HTMLImageElement]>>action;
            const [kind, image] = da.data;
            const newState = {
                ...state,
                images: {
                    ...state.images,
                    pieces: new Map<PieceKind, HTMLImageElement>(state.images.pieces)
                }
            }
            newState.images.pieces.set(kind, image);
            return newState;
        }
        //BoardScroll and BoardZoom must be handled at a higher level because they update the boardview
        default:
            return state;
    }
}