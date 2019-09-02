import { CustomAction, DataAction } from "./root";
import { Game, Event, StateAndEventResponse, SnapshotInfo } from "../api/model";
import { BoardView } from "../viewModel/board/model";
import GameHistory from "../viewModel/gameHistory";

export interface State {
    game : Game,
    history : Event[],
    boardView : BoardView,
    snapshots : SnapshotInfo[]
}

export const defaultState : State = {
    game: null,
    history: null,
    boardView: null,
    snapshots: null
};

export enum ActionTypes {
    CreateGame = "CREATE_GAME",
    LoadGame = "LOAD_GAME",
    ClearGame = "CLEAR_GAME",
    LoadGameHistory = "LOAD_GAME_HISTORY",
    UpdateGame = "UPDATE_GAME",
    LoadSnapshots = "LOAD_SNAPSHOTS",
    SnapshotSaved = "SNAPSHOT_SAVED",
    SnapshotDeleted = "SNAPSHOT_DELETED"
}

export class Actions {
    public static createGame(game : Game) {
        return {
            type: ActionTypes.CreateGame,
            data: game
        };
    }

    public static loadGame(game : Game) {
        return {
            type: ActionTypes.LoadGame,
            data: game
        };
    }

    public static clearGame() {
        return {
            type: ActionTypes.ClearGame
        };
    }

    public static loadGameHistory(history : Event[]) {
        return {
            type: ActionTypes.LoadGameHistory,
            data: history
        };
    }

    public static updateGame(response : StateAndEventResponse) {
        return {
            type: ActionTypes.UpdateGame,
            data: response
        };
    }

    public static loadSnapshots(snapshots : SnapshotInfo[]) {
        return {
            type: ActionTypes.LoadSnapshots,
            data: snapshots
        };
    }

    public static snapshotSaved(snapshot : SnapshotInfo) {
        return {
            type: ActionTypes.SnapshotSaved,
            data: snapshot
        };
    }

    public static snapshotDeleted(snapshotId : number) {
        return {
            type: ActionTypes.SnapshotDeleted,
            data: snapshotId
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.CreateGame: {
            const da = <DataAction<Game>>action;
            const newState = {...defaultState};
            newState.game = da.data;
            return newState;
        }
        case ActionTypes.ClearGame: {
            return defaultState;
        }
        case ActionTypes.LoadGameHistory: {
            const da = <DataAction<Event[]>>action;
            const newState = {...state};
            const history = da.data.filter(e => GameHistory.isDisplayableEvent(e));
            newState.history = history;
            return newState;
        }
        case ActionTypes.LoadSnapshots: {
            const da = <DataAction<SnapshotInfo[]>>action;
            const newState = {...state};
            newState.snapshots = da.data;
            return newState;
        }
        case ActionTypes.SnapshotSaved: {
            const da = <DataAction<SnapshotInfo>>action;
            const newState = {...state};
            newState.snapshots = state.snapshots ? state.snapshots.concat(da.data) : [da.data];
            return newState;
        }
        case ActionTypes.SnapshotDeleted: {
            const da = <DataAction<number>>action;
            const newState = {...state};
            newState.snapshots = state.snapshots.filter(s => s.id !== da.data);
            return newState;
        }
        //Load game and update game must be at a higher level because they update multiple areas of the store
        default:
            return state;
    }
}