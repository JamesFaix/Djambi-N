import { CustomAction } from "./root";
import { Board } from "../api/model";

export interface State {
    boards : Map<number, Board>
}

export const defaultState : State = {
    boards: new Map<number, Board>()
};

export enum ActionTypes {
    LoadBoard = "LOAD_BOARD",
}

export class Actions {
    public static loadBoard(board : Board) {
        return {
            type: ActionTypes.LoadBoard,
            data: board
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        //LoadBoard must be handled at a higher leverl because it updates the BoardView
        default:
            return state;
    }
}