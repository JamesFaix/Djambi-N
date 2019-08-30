import { CustomAction, DataAction } from "./root";
import { Game, Event, StateAndEventResponse } from "../api/model";
import { BoardView } from "../viewModel/board/model";
import GameHistory from "../viewModel/gameHistory";

export interface State {
    game : Game,
    history : Event[],
    boardView : BoardView
}

export const defaultState : State = {
    game: null,
    history: null,
    boardView: null
};

export enum ActionTypes {
    CreateGame = "CREATE_GAME",
    LoadGame = "LOAD_GAME",
    LoadGameHistory = "LOAD_GAME_HISTORY",
    UpdateGame = "UPDATE_GAME"
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
        case ActionTypes.LoadGameHistory: {
            const da = <DataAction<Event[]>>action;
            const newState = {...state};
            const history = da.data.filter(e => GameHistory.isDisplayableEvent(e));
            newState.history = history;
            return newState;
        }
        //Load game and update game must be at a higher level because they update multiple areas of the store
        default:
            return state;
    }
}