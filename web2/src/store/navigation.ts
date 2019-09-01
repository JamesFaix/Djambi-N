import { CustomAction, DataAction } from "./root";

export interface State {
    enableLogin ?: boolean,
    enableSignup ?: boolean,
    enableDashboard ?: boolean,
    enableCreateGame ?: boolean,
    enableLobby ?: boolean,
    enablePlay ?: boolean,
    enableDiplomacy ?: boolean,
    enableSnapshots ?: boolean,
    gameId ?: number
}

export const defaultState : State = {
    //Intentionally empty
};

enum ActionTypes {
    SetNavigationOptions = "SET_NAVIGATION_OPTIONS"
}

export class Actions {
    public static setNavigationOptions (state : State) {
        return {
            type: ActionTypes.SetNavigationOptions,
            data : state
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.SetNavigationOptions: {
            const da = <DataAction<State>>action;
            return da.data;
        }
        default:
            return state;
    }
}