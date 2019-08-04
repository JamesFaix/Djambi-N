import { CustomAction, DataAction, ActionStatus, ActionTypes } from './actions';
import { Session, Game, Event } from '../api/model';
import { AppState, defaultState } from './state';

export function reducer(state: AppState, action : CustomAction) : AppState {
    switch (action.type) {
        case ActionTypes.Login:
            return loginReducer(state, action);
        case ActionTypes.Logout:
            return logoutReducer(state, action);
        case ActionTypes.Signup:
            return signupReducer(state, action);
        case ActionTypes.LoadGame:
            return loadGameReducer(state, action);
        case ActionTypes.LoadHistory:
            return loadGameHistoryReducer(state, action);
        case ActionTypes.UpdateGame:
            return updateGameReducer(state, action);
        default:
            return state;
    }
}

function loginReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests.loginPending = true;
            break;

        case ActionStatus.Success:
            let da = <DataAction<Session>>action;
            newState = {...state};
            newState.requests.loginPending = false;
            newState.session = da.data;
            break;

        case ActionStatus.Error:
            throw "Not yet implemented.";

        default:
            throw "Not supported.";
    }

    return newState;
}

function logoutReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests.logoutPending = true;
            break;

        case ActionStatus.Success:
            newState = defaultState();
            break;

        case ActionStatus.Error:
            throw "Not yet implemented.";

        default:
            throw "Not supported.";
    }

    return newState;
}

function signupReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests.signupPending = true;
            break;

        case ActionStatus.Success:
            newState = {...state};
            newState.requests.signupPending = false;
            break;

        case ActionStatus.Error:
            throw "Not yet implemented.";

        default:
            throw "Not supported.";
    }

    return newState;
}

function loadGameReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests.loadGamePending = true;
            break;

        case ActionStatus.Success:
            let da = <DataAction<Game>>action;
            newState = {...state};
            newState.requests.loadGamePending = false;
            newState.currentGame.game = da.data;
            newState.currentGame.history = null;
            break;

        case ActionStatus.Error:
            throw "Not yet implemented.";

        default:
            throw "Not supported.";
    }

    return newState;
}

function loadGameHistoryReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests.loadGameHistoryPending = true;
            break;

        case ActionStatus.Success:
            let da = <DataAction<Event[]>>action;
            newState = {...state};
            newState.requests.loadGameHistoryPending = false;
            newState.currentGame.history = da.data;
            break;

        case ActionStatus.Error:
            throw "Not yet implemented.";

        default:
            throw "Not supported.";
    }

    return newState;
}

function updateGameReducer(state: AppState, action : CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Success:
            let da = <DataAction<[Game, Event]>>action;
            newState = {...state};
            newState.currentGame.game = da.data[0];

            let newHistory = state.currentGame.history.slice();
            newHistory.push(da.data[1]);
            newState.currentGame.history = newHistory;
            break;

        default:
            throw "Not supported.";
    }

    return newState;
}