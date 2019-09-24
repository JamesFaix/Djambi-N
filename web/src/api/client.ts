/*
 * This file was generated with the Client Generator utility.
 * Do not manually edit.
 */

import * as Model from './model';
import {ApiClientCore, HttpMethod} from './clientCore';

//-------- USER --------

export function createUser(request : Model.CreateUserRequest) : Promise<Model.User> {
    const route = `/users`;
    return ApiClientCore.sendRequest<Model.CreateUserRequest, Model.User>(
        HttpMethod.Post, route, request);
}

export function deleteUser(userId : number) : Promise<{}> {
    const route = `/users/${userId}`;
    return ApiClientCore.sendRequest<{}, {}>(
        HttpMethod.Delete, route);
}

export function getCurrentUser() : Promise<Model.User> {
    const route = `/users/current`;
    return ApiClientCore.sendRequest<{}, Model.User>(
        HttpMethod.Get, route);
}

export function getUser(userId : number) : Promise<Model.User> {
    const route = `/users/${userId}`;
    return ApiClientCore.sendRequest<{}, Model.User>(
        HttpMethod.Get, route);
}

//-------- SESSION --------

export function login(request : Model.LoginRequest) : Promise<Model.Session> {
    const route = `/sessions`;
    return ApiClientCore.sendRequest<Model.LoginRequest, Model.Session>(
        HttpMethod.Post, route, request);
}

export function logout() : Promise<{}> {
    const route = `/sessions`;
    return ApiClientCore.sendRequest<{}, {}>(
        HttpMethod.Delete, route);
}

//-------- BOARD --------

export function getBoard(regionCount : number) : Promise<Model.Board> {
    const route = `/boards/${regionCount}`;
    return ApiClientCore.sendRequest<{}, Model.Board>(
        HttpMethod.Get, route);
}

export function getCellPaths(regionCount : number, cellId : number) : Promise<number[][]> {
    const route = `/boards/${regionCount}/cells/${cellId}/paths`;
    return ApiClientCore.sendRequest<{}, number[][]>(
        HttpMethod.Get, route);
}

//-------- GAME --------

export function createGame(parameters : Model.GameParameters) : Promise<Model.Game> {
    const route = `/games`;
    return ApiClientCore.sendRequest<Model.GameParameters, Model.Game>(
        HttpMethod.Post, route, parameters);
}

export function getGame(gameId : number) : Promise<Model.Game> {
    const route = `/games/${gameId}`;
    return ApiClientCore.sendRequest<{}, Model.Game>(
        HttpMethod.Get, route);
}

export function startGame(gameId : number) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/start-request`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Post, route);
}

export function updateGameParameters(gameId : number, parameters : Model.GameParameters) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/parameters`;
    return ApiClientCore.sendRequest<Model.GameParameters, Model.StateAndEventResponse>(
        HttpMethod.Put, route, parameters);
}

//-------- PLAYER --------

export function addPlayer(gameId : number, request : Model.CreatePlayerRequest) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/players`;
    return ApiClientCore.sendRequest<Model.CreatePlayerRequest, Model.StateAndEventResponse>(
        HttpMethod.Post, route, request);
}

export function removePlayer(gameId : number, playerId : number) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/players/${playerId}`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Delete, route);
}

export function updatePlayerStatus(gameId : number, playerId : number, status : Model.PlayerStatus) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/players/${playerId}/status/${status}`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Put, route);
}

//-------- TURN --------

export function commitTurn(gameId : number) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/current-turn/commit-request`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Post, route);
}

export function resetTurn(gameId : number) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/current-turn/reset-request`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Post, route);
}

export function selectCell(gameId : number, cellId : number) : Promise<Model.StateAndEventResponse> {
    const route = `/games/${gameId}/current-turn/selection-request/${cellId}`;
    return ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
        HttpMethod.Post, route);
}

//-------- EVENTS --------

export function getEvents(gameId : number, query : Model.EventsQuery) : Promise<Model.Event[]> {
    const route = `/games/${gameId}/events/query`;
    return ApiClientCore.sendRequest<Model.EventsQuery, Model.Event[]>(
        HttpMethod.Post, route, query);
}

//-------- SNAPSHOTS --------

export function createSnapshot(gameId : number, request : Model.CreateSnapshotRequest) : Promise<Model.SnapshotInfo> {
    const route = `/games/${gameId}/snapshots`;
    return ApiClientCore.sendRequest<Model.CreateSnapshotRequest, Model.SnapshotInfo>(
        HttpMethod.Post, route, request);
}

export function deleteSnapshot(gameId : number, snapshotId : number) : Promise<{}> {
    const route = `/games/${gameId}/snapshots/${snapshotId}`;
    return ApiClientCore.sendRequest<{}, {}>(
        HttpMethod.Delete, route);
}

export function getSnapshotsForGame(gameId : number) : Promise<Model.SnapshotInfo[]> {
    const route = `/games/${gameId}/snapshots`;
    return ApiClientCore.sendRequest<{}, Model.SnapshotInfo[]>(
        HttpMethod.Get, route);
}

export function loadSnapshot(gameId : number, snapshotId : number) : Promise<{}> {
    const route = `/games/${gameId}/snapshots/${snapshotId}/load-request`;
    return ApiClientCore.sendRequest<{}, {}>(
        HttpMethod.Post, route);
}

//-------- SEARCH --------

export function searchGames(query : Model.GamesQuery) : Promise<Model.SearchGame[]> {
    const route = `/search/games`;
    return ApiClientCore.sendRequest<Model.GamesQuery, Model.SearchGame[]>(
        HttpMethod.Post, route, query);
}

