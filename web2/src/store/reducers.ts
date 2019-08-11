import { CustomAction, DataAction, ActionStatus, ActionTypes } from './actions';
import { Game, GamesQuery, User, GameParameters, CreatePlayerRequest, Event } from '../api/model';
import { AppState, StateFactory, NavigationState } from './state';

export function reducer(state: AppState, action : CustomAction) : AppState {
    switch (action.type) {
        //Session actions
        case ActionTypes.Login:
            return loginReducer(state, action);
        case ActionTypes.Logout:
            return logoutReducer(state, action);
        case ActionTypes.Signup:
            return signupReducer(state, action);
        case ActionTypes.RestoreSession:
                return restoreSessionReducer(state, action);

        //Game search actions
        case ActionTypes.QueryGames:
            return queryGamesReducer(state, action);
        case ActionTypes.UpdateGamesQuery:
            return updateGamesQueryReducer(state, action);

        //Game creation actions
        case ActionTypes.CreateGame:
            return createGameReducer(state, action);
        case ActionTypes.UpdateCreateGameForm:
            return updateCreateGameFormReducer(state, action);

        //Game actions
        case ActionTypes.LoadGame:
            return loadGameReducer(state, action);
        case ActionTypes.LoadGameHistory:
            return loadGameHistoryReducer(state, action);
        case ActionTypes.AddPlayer:
            return addPlayerReducer(state, action);
        case ActionTypes.RemovePlayer:
            return removePlayerReducer(state, action);
        case ActionTypes.StartGame:
            return startGameReducer(state, action);

        //Misc
        case ActionTypes.SetNavigationOptions:
            return setNavigationOptionsReducer(state, action);

        default:
            return state;
    }
}

//#region Session actions

function loginReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.loginPending = true;
            return newState;
        }

        case ActionStatus.Success: {
            const da = <DataAction<User>>action;
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.loginPending = false;
            newState.session.user = da.data;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}

function logoutReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending:
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.logoutPending = true;
            return newState;

        case ActionStatus.Success:
            //This is the one case where the normal combineReducers pattern doesn't make sense
            return StateFactory.defaultAppState();

        default:
            throw "Unsupported case: " + action.status;
    }
}

function signupReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.signupPending = true;
            return newState;
        }

        case ActionStatus.Success: {
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.signupPending = false;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}

function restoreSessionReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.restoreSessionPending = true;
            return newState;
        }

        case ActionStatus.Success: {
            const da = <DataAction<User>>action;
            const newState = {...state};
            newState.session = {...state.session};
            newState.session.restoreSessionPending = false;
            newState.session.user = da.data;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}

//#endregion

//#region Game search actions

function queryGamesReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const da = <DataAction<GamesQuery>>action;
            const newState = {...state};
            newState.gamesQuery = {
                queryPending: true,
                query: da.data,
                results: null
            };
            return newState;
        }

        case ActionStatus.Success: {
            const da = <DataAction<Game[]>>action;
            const newState = {...state};
            newState.gamesQuery = {
                queryPending: false,
                query: state.gamesQuery.query,
                results: da.data
            };
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}

function updateGamesQueryReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Success: {
            const da = <DataAction<GamesQuery>>action;
            const newState = {...state};
            newState.gamesQuery = {...state.gamesQuery};
            newState.gamesQuery.query = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}
//#endregion

//#region Game creation actions

function updateCreateGameFormReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Success: {
            const da = <DataAction<GameParameters>>action;
            const newState = {...state};
            newState.createGameForm = {...state.createGameForm};
            newState.createGameForm.parameters = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function createGameReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.createGameForm = {...state.createGameForm};
            newState.createGameForm.createGamePending = true;
            return newState;
        }

        case ActionStatus.Success: {
            const da = <DataAction<Game>>action;
            const newState = {...state};
            newState.createGameForm = {...state.createGameForm};
            newState.createGameForm.createGamePending = false;
            newState.activeGame = StateFactory.defaultActiveGameState();
            newState.activeGame.game = da.data;
            return newState;
        }

        default:
            throw "Unsupported case: " + action.status;
    }
}
//#endregion

//#region Game actions

function loadGameReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.activeGame = StateFactory.defaultActiveGameState();
            newState.activeGame.loadGamePending = true;
            return newState;
        }
        case ActionStatus.Success: {
            const da = <DataAction<Game>>action;
            const newState = {...state};
            newState.activeGame = StateFactory.defaultActiveGameState();
            newState.activeGame.game = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function loadGameHistoryReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.loadHistoryPending = true;
            return newState;
        }
        case ActionStatus.Success: {
            const da = <DataAction<Event[]>>action;
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.loadHistoryPending = false;
            newState.activeGame.history = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function addPlayerReducer(state: AppState, action : CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.addPlayerPending = true;
            return newState;
        }
        case ActionStatus.Success: {
            const da = <DataAction<Game>>action;
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.addPlayerPending = false;
            newState.activeGame.game = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function removePlayerReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.removePlayerPending = true;
            return newState;
        }
        case ActionStatus.Success: {
            const da = <DataAction<Game>>action;
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.removePlayerPending = false;
            newState.activeGame.game = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

function startGameReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Pending: {
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.startGamePending = true;
            return newState;
        }
        case ActionStatus.Success: {
            const da = <DataAction<Game>>action;
            const newState = {...state};
            newState.activeGame = {...state.activeGame};
            newState.activeGame.startGamePending = false;
            newState.activeGame.game = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}

//#endregion

function setNavigationOptionsReducer(state: AppState, action: CustomAction) : AppState {
    switch (action.status) {
        case ActionStatus.Success: {
            const da = <DataAction<NavigationState>>action;
            const newState = {...state};
            newState.navigation = da.data;
            return newState;
        }
        default:
            throw "Unsupported case: " + action.status;
    }
}
