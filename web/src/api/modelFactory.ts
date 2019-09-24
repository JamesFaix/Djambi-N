import { GamesQuery, LoginRequest, CreateUserRequest, GameParameters } from "./model";

export function emptyGamesQuery() : GamesQuery {
    return {
        gameId: null,
        descriptionContains: null,
        createdByUserName: null,
        playerUserName: null,
        isPublic: null,
        allowGuests: null,
        statuses: [],
        createdBefore: null,
        createdAfter: null,
        lastEventBefore: null,
        lastEventAfter: null
    };
}

export function loginRequestFromCreateUserRequest(request: CreateUserRequest) : LoginRequest {
    return {
        username: request.name,
        password: request.password
    };
}

export function defaultGameParameters() : GameParameters {
    return {
        regionCount: 3,
        description: "",
        allowGuests: true,
        isPublic: true
    };
}