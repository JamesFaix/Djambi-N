import { CustomAction, DataAction } from "./root";
import { PieceKind } from "../api/model";

export interface State {
    pieces : Map<PieceKind, HTMLImageElement>
}

export const defaultState : State = {
    pieces: new Map<PieceKind, HTMLImageElement>()
};

export enum ActionTypes {
    LoadPieceImage = "LOAD_PIECE_IMAGE"
}

export class Actions {
    public static loadPieceImage(pieceKind : PieceKind, image : HTMLImageElement) {
        return {
            type: ActionTypes.LoadPieceImage,
            data: [pieceKind, image]
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.LoadPieceImage: {
            const da = <DataAction<[PieceKind, HTMLImageElement]>>action;
            const [kind, image] = da.data;
            const newState = {...state};
            const pieces = new Map<PieceKind, HTMLImageElement>(state.pieces);
            pieces.set(kind, image);
            newState.pieces = pieces;
            return newState;
        }
        default:
            return state;
    }
}