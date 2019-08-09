import { CustomAction, DataAction, ActionStatus, ActionTypes } from './actions';
import { Game, GamesQuery, User } from '../api/model';
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
        case ActionTypes.QueryGames:
            return queryGamesReducer(state, action);
        case ActionTypes.UpdateGamesQuery:
            return updateGamesQueryReducer(state, action);
        case ActionTypes.RestoreSession:
            return restoreSessionReducer(state, action);
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
            let da = <DataAction<User>>action;
            const newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.loginPending = false;
            newState.user = da.data;
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

function loadGameReducer(state: AppState, action: CustomAction) : AppState {
    let newState = state;

    switch (action.status) {
        case ActionStatus.Pending:
            newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.loadGamePending = true;
            break;

        case ActionStatus.Success:
            let da = <DataAction<Game>>action;
            newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.loadGamePending = false;
            newState.currentGame = {
                game: da.data,
                history: null
            };
            break;

        default:
            throw "Unsupported case: " + action.status;
    }

    return newState;
}

function queryGamesReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            let da = <DataAction<GamesQuery>>action;
            let newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.gamesQueryPending = true;
            newState.gamesQuery = {
                query: da.data,
                results: null
            };
            return newState;
        }

        case ActionStatus.Success: {
            let da = <DataAction<Game[]>>action;
            let newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.gamesQueryPending = false;
            newState.gamesQuery.results = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function updateGamesQueryReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Success: {
            let da = <DataAction<GamesQuery>>action;
            let newState = {...state};
            if (!newState.gamesQuery) {
                newState.gamesQuery = {
                    query: null,
                    results: null
                };
            }
            newState.gamesQuery.query = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function restoreSessionReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            let newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.restoreSessionPending = true;
            return newState;
        }

        case ActionStatus.Success: {
            let da = <DataAction<User>>action;
            let newState = {...state};
            newState.requests = {...state.requests};
            newState.requests.restoreSessionPending = false;
            newState.user = da.data;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}