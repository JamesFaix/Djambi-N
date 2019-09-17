import { CustomAction, DataAction } from "./root";

export enum NotificationType {
    Error,
    Info
}

export interface NotificationInfo {
    id : string,
    message : string,
    type : NotificationType
}

export interface State {
    items: NotificationInfo[]
}

export const defaultState : State = {
    items: []
}

export enum ActionTypes {
    AddNotification = "ADD_NOTIFICATION",
    RemoveNotification = "REMOVE_NOTIFICATION"
}

export class Actions {
    public static addNotification(info : NotificationInfo) {
        return {
            type: ActionTypes.AddNotification,
            data: info
        };
    }

    public static removeNotification(id : string) {
        return {
            type: ActionTypes.RemoveNotification,
            data: id
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type) {
        case ActionTypes.AddNotification: {
            const da = <DataAction<NotificationInfo>>action;
            return {
                ...state,
                items: [...state.items, da.data]
            };
        }
        case ActionTypes.RemoveNotification: {
            const da = <DataAction<string>>action;
            return {
                ...state,
                items: state.items.filter(x => x.id !== da.data)
            }
        }
        default:
            return state;
    }
}