import { CustomAction, DataAction } from "./root";
import { GamesQuery, SearchGame } from "../api/model";
import * as ModelFactory from '../api/modelFactory';

export interface State {
    query : GamesQuery,
    results : SearchGame[],
    recent : SearchGame[]
}

export const defaultState : State = {
    query: ModelFactory.emptyGamesQuery(),
    results: [],
    recent: []
};

enum ActionTypes {
    UpdateGamesQuery = "UPDATE_GAMES_QUERY",
    LoadSearchResults = "LOAD_SEARCH_RESULTS",
    LoadRecentGames = "LOAD_RECENT_GAMES"
}

export class Actions {
    public static updateGamesQuery(query : GamesQuery) {
        return {
            type: ActionTypes.UpdateGamesQuery,
            data: query
        };
    }

    public static loadSearchResults(games : SearchGame[]) {
        return {
            type: ActionTypes.LoadSearchResults,
            data: games
        }
    }

    public static loadRecentGames(games : SearchGame[]) {
        return {
            type: ActionTypes.LoadRecentGames,
            data: games
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.UpdateGamesQuery: {
            const da = <DataAction<GamesQuery>>action;
            return {
                ...state,
                query: da.data
            };
        }
        case ActionTypes.LoadSearchResults: {
            const da = <DataAction<SearchGame[]>>action;
            return {
                ...state,
                results: da.data
            };
        }
        case ActionTypes.LoadRecentGames: {
            const da = <DataAction<SearchGame[]>>action;
            return {
                ...state,
                recent: da.data
            };
        }
        default:
            return state;
    }
}