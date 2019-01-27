import * as Model from './model';
import {ApiClientCore, HttpMethod} from './clientCore';

export default class ApiClient {

    //Users
    async getCurrentUser() : Promise<Model.User> {
        return await ApiClientCore.sendRequest<{}, Model.User>(
            HttpMethod.Get, "/users/current");
    }

    async createUser(request : Model.CreateUserRequest) : Promise<Model.User> {
        return await ApiClientCore.sendRequest<Model.CreateUserRequest, Model.User>(
            HttpMethod.Post, "/users", request);
    }

    //Session
    async login(request : Model.LoginRequest) : Promise<Model.Session> {
        return await ApiClientCore.sendRequest<Model.LoginRequest, Model.Session>(
            HttpMethod.Post, "/sessions", request);
    }

    async logout() : Promise<{}> {
        return await ApiClientCore.sendRequest<{}, {}>(
            HttpMethod.Delete, "/sessions");
    }

    //Game
    async createGame(request : Model.GameParameters) : Promise<Model.Game> {
        return await ApiClientCore.sendRequest<Model.GameParameters, Model.Game>(
            HttpMethod.Post, "/games", request);
    }

    async getGame(gameId : number) : Promise<Model.Game> {
        return await ApiClientCore.sendRequest<{}, Model.Game>(
            HttpMethod.Get, "/games/" + gameId);
    }

    async getGames(query : Model.GamesQuery) : Promise<Model.Game[]> {
        return await ApiClientCore.sendRequest<Model.GamesQuery, Model.Game[]>(
            HttpMethod.Post, "/games/query", query);
    }

    async removePlayer(gameId : number, playerId : number) : Promise<Model.StateAndEventResponse> {
        return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
            HttpMethod.Delete, "/games/" + gameId + "/players/" + playerId);
    }

    async addPlayer(gameId : number, request : Model.CreatePlayerRequest) : Promise<Model.StateAndEventResponse> {
        return await ApiClientCore.sendRequest<Model.CreatePlayerRequest, Model.StateAndEventResponse>(
            HttpMethod.Post, "/games/" + gameId + "/players", request);
    }

    async startGame(gameId : number) : Promise<Model.StateAndEventResponse> {
        return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
            HttpMethod.Post, "/games/" + gameId + "/start-request");
    }
}