import { GamesQuery, LoginRequest, CreateUserRequest, GameParameters, GameStatus } from "./model";

export function emptyGamesQuery() : GamesQuery {
    return {
        gameId: null,
        descriptionContains: null,
        createdByUserName: null,
        playerUserName: null,
        containsMe: false, //Default for search is games w/o you, default for dashboard is games w/ you
        isPublic: null,
        allowGuests: null,
        statuses: [ GameStatus.Pending, GameStatus.InProgress ],
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