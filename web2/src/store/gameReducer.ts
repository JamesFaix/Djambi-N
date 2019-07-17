import { Event, Game } from '../api/model';

export interface GameState {
    game : Game,
    history : Event[]
}

export const LOAD_GAME = "LOAD_GAME";
export const UPDATE_GAME = "UPDATE_GAME";
export const LOAD_HISTORY = "LOAD_HISTORY";

interface LoadGameAction {
    type : typeof LOAD_GAME,
    game : Game
}

interface LoadHistoryAction {
    type : typeof LOAD_HISTORY,
    history : Event[]
}

interface UpdateGameAction {
    type : typeof UPDATE_GAME,
    game : Game,
    event : Event
}

export type GameActionTypes =
    LoadGameAction |
    LoadHistoryAction |
    UpdateGameAction;

export function loadGame(game : Game) : LoadGameAction {
    return {
        type: LOAD_GAME,
        game: game
    };
}

export function loadHistory(history : Event[]) : LoadHistoryAction {
    return {
        type: LOAD_HISTORY,
        history: history
    };
}

export function updateGame(updateGame : Game, event : Event) : UpdateGameAction {
    return {
        type: UPDATE_GAME,
        game : updateGame,
        event: event
    };
}

export function gameReducer(
    state: GameState,
    action: GameActionTypes) : GameState {

    switch (action.type) {
        case LOAD_GAME:
            return {
                game: action.game,
                history: []
            };
        case LOAD_HISTORY:
            return {
                game: state.game,
                history: action.history
            };
        case UPDATE_GAME:
            return {
                game: action.game,
                history: state.history.concat(action.event)
            };
        default:
            return state;
    }
}