import { CustomAction, DataAction } from "./root";
import { MapUtil } from "../utilities/collections";

export interface State {
    errors: Map<string, string>
}

export const defaultState : State = {
    errors: new Map<string, string>()
}

export enum ActionTypes {
    Error = "ERROR",
    ErrorExpired = "ERROR_EXPIRED"
}

export class Actions {
    public static error(key : string, message : string) {
        return {
            type: ActionTypes.Error,
            data: [key, message]
        }
    }

    public static errorExpired(key : string) {
        return {
            type: ActionTypes.ErrorExpired,
            data: key
        }
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type) {
        case ActionTypes.Error: {
            const da = <DataAction<[string, string]>>action;
            const [key, message] = da.data;
            return {
                ...state,
                errors: MapUtil.add(state.errors, key, message)
            };
        }
        case ActionTypes.ErrorExpired: {
            const da = <DataAction<string>>action;
            return {
                ...state,
                errors: MapUtil.remove(state.errors, da.data)
            }
        }
        default:
            return state;
    }
}