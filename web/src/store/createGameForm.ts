import { CustomAction, DataAction } from "./root";
import { GameParameters } from "../api/model";
import * as ModelFactory from '../api/modelFactory';

export interface State {
    parameters : GameParameters
}

export const defaultState : State = {
    parameters: ModelFactory.defaultGameParameters()
};

enum ActionTypes {
    UpdateCreateGameForm = "UPDATE_CREATE_GAME_FORM"
}

export class Actions {
    public static updateCreateGameForm(parameters: GameParameters) {
        return {
            type: ActionTypes.UpdateCreateGameForm,
            data: parameters
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.UpdateCreateGameForm: {
            const da = <DataAction<GameParameters>>action;
            const newState = {...state};
            newState.parameters = da.data;
            return newState;
        }
        default:
            return state;
    }
}