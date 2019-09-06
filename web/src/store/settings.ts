import { DataAction, CustomAction } from "./root";
import { DebugSettings, defaultDebugSettings } from "../debug";

export interface State {
    debug : DebugSettings,
}

export const defaultState : State = {
    debug: defaultDebugSettings,
};

export enum ActionTypes {
    ApplyDebugSettings = "APPLY_DEBUG_SETTINGS"
}

export class Actions {
    public static applyDebugSettings(settings : DebugSettings) {
        return {
            type: ActionTypes.ApplyDebugSettings,
            data: settings
        }
    };
}

export function reducer(state: State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.ApplyDebugSettings: {
            const da = <DataAction<DebugSettings>>action;
            return {
                ...state,
                debug: da.data
            };
        }
        default:
            return state;
    }
}