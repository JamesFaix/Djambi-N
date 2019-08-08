import { CustomAction, DataAction, ActionStatus, ActionTypes } from './actions';
import { Session } from '../api/model';
import { AppState, defaultState } from './state';

export function reducer(state: AppState, action : CustomAction) : AppState {
    switch (action.type) {
        case ActionTypes.Login:
            return loginReducer(state, action);
        case ActionTypes.Logout:
            return logoutReducer(state, action);
        case ActionTypes.Signup:
            return signupReducer(state, action);
        default:
            return state;
    }
}

function loginReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.loginPending = true;
            return newState;
        }

        case ActionStatus.Success: {
            let da = <DataAction<Session>>action;
            const newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.loginPending = false;
            newState.session = da.data;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}

function logoutReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.logoutPending = true;
            break;

        case ActionStatus.Success:
            newState = defaultState();
            break;

        default:
            throw "Unsupported case: " + action.status;
    }

    return newState;
}

function signupReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.signupPending = true;
            break;

        case ActionStatus.Success:
            newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.signupPending = false;
            break;

        default:
            throw "Unsupported case: " + action.status;
    }

    return newState;
}