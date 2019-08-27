import { CustomAction, DataAction } from "./root";
import { ApiRequest, ApiResponse, ApiError } from "../api/requestModel";

export interface State {
    openRequests : ApiRequest[]
}

export const defaultState : State = {
    openRequests : []
};

export enum ActionTypes {
    ApiRequest = "API_REQUEST",
    ApiResponse = "API_RESPONSE",
    ApiError = "API_ERROR",
}

export class Actions {
    public static apiRequest(request: ApiRequest) {
        return {
            type: ActionTypes.ApiRequest,
            data: request
        };
    }

    public static apiResponse(response : ApiResponse) {
        return {
            type: ActionTypes.ApiResponse,
            data: response
        };
    }

    public static apiError(error : ApiError) {
        return {
            type: ActionTypes.ApiError,
            data: error
        };
    }
}

export function reducer(state : State, action : CustomAction) : State {
    if (!state) { state = {...defaultState}; }

    switch (action.type){
        case ActionTypes.ApiRequest: {
            const da = <DataAction<ApiRequest>>action;
            const newState = {...state};
            newState.openRequests = newState.openRequests.concat([da.data]);
            return newState;
        }
        case ActionTypes.ApiResponse: {
            const da = <DataAction<ApiResponse>>action;
            const newState = {...state};
            newState.openRequests = state.openRequests.filter(r => r.id !== da.data.requestId);
            return newState;
        }
        case ActionTypes.ApiError: {
            const da = <DataAction<ApiError>>action;
            const newState = {...state};
            newState.openRequests = state.openRequests.filter(r => r.id !== da.data.requestId);
            return newState;
        }
        default:
            return state;
    }
}