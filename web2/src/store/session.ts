import { CustomAction, DataAction } from "./root";
import { User } from "../api/model";

export interface State {
    user : User
}

export const defaultState : State = {
    user : null
};

export enum ActionTypes {
    Login = "LOGIN",
    Logout = "LOGOUT",
    Signup = "SIGNUP",
    RestoreSession = "RESTORE_SESSION",
}

export class Actions {
    public static login(user : User) {
        return {
            type: ActionTypes.Login,
            data: user
        };
    }

    public static logout() {
        return {
            type: ActionTypes.Logout,
        };
    }

    public static signup(user : User) {
        return {
            type: ActionTypes.Signup,
            data: user
        };
    }

    public static restoreSession(user : User) {
        return {
            type: ActionTypes.RestoreSession,
            data: user
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.Login: {
            const da = <DataAction<User>>action;
            const newState = {...state};
            newState.user = da.data;
            return newState;
        }
        case ActionTypes.Signup: {
            return state;
        }
        case ActionTypes.RestoreSession: {
            const da = <DataAction<User>>action;
            const newState = {...state};
            newState.user = da.data;
            return newState;
        }
        //Logout must be done at a higher level because it affects all state
        default:
            return state;
    }
}