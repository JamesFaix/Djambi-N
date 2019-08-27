import { CustomAction, DataAction } from "./root";
import { Game, GamesQuery } from "../api/model";
import * as ModelFactory from '../api/modelFactory';

export interface State {
    query : GamesQuery,
    results : Game[]
}

export const defaultState : State = {
    query: ModelFactory.emptyGamesQuery(),
    results: []
};

enum ActionTypes {
    UpdateGamesQuery = "UPDATE_GAMES_QUERY",
    QueryGames = "QUERY_GAMES"
}

export class Actions {
    public static updateGamesQuery(query : GamesQuery) {
        return {
            type: ActionTypes.UpdateGamesQuery,
            data: query
        };
    }

    public static queryGames(games : Game[]) {
        return {
            type: ActionTypes.QueryGames,
            data: games
        }
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.UpdateGamesQuery: {
            const da = <DataAction<GamesQuery>>action;
            const newState = {...state};
            newState.query = da.data;
            return newState;
        }
        case ActionTypes.QueryGames: {
            const da = <DataAction<Game[]>>action;
            const newState = {...state};
            newState.results = da.data;
            return newState;
        }
        default:
            return state;
    }
}