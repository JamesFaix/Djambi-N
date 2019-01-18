import * as Model from './model';
import Environment from "../environment";

enum HttpMethod {
    Get = "GET",
    Put = "PUT",
    Post = "POST",
    Delete = "DELETE"
}

export default class ApiClient {

    private async sendRequest<TBody, TResponse>(
        method : HttpMethod,
        route : string,
        body : TBody = null)
        : Promise<TResponse> {

        const url = Environment.apiAddress() + route;
        const endpointDescription = method.toString() + " " + route

        const fetchParams : RequestInit = {
            method: method.toString(),
            credentials: "include",
            mode: "cors"
        };

        if (body !== null) {
            fetchParams.headers=  {
                "Content-Type": "application/json",
            };
            fetchParams.body = JSON.stringify(body);
        }

        return await fetch(url, fetchParams)
            .then(async response => {
                if (!response.ok){
                    return response.json()
                        .then(errorMessage => {
                            console.log(endpointDescription + " failed (" + errorMessage + ")");
                            return null;

                        });
                } else {
                    console.log(endpointDescription + " succeeded");
                    return await response.json();
                }
            })
            .catch(reason => {
                console.log(reason);
            });
    }

    //Users
    async getCurrentUser() : Promise<Model.User> {
        return await this.sendRequest<{}, Model.User>(
            HttpMethod.Get, "/users/current");
    }

    async createUser(request : Model.CreateUserRequest) : Promise<Model.User> {
        return await this.sendRequest<Model.CreateUserRequest, Model.User>(
            HttpMethod.Post, "/users", request);
    }

    //Session
    async login(request : Model.LoginRequest) : Promise<Model.User> {
        return await this.sendRequest<Model.LoginRequest, Model.User>(
            HttpMethod.Post, "/sessions", request);
    }

    async logout() : Promise<{}> {
        return await this.sendRequest<{}, {}>(
            HttpMethod.Delete, "/sessions");
    }

    //Game
    async createGame(request : Model.GameParameters) : Promise<Model.Game> {
        return await this.sendRequest<Model.GameParameters, Model.Game>(
            HttpMethod.Post, "/games", request);
    }

    async getGame(gameId : number) : Promise<Model.Game> {
        return await this.sendRequest<{}, Model.Game>(
            HttpMethod.Get, "/games/" + gameId);
    }

    async getGames(query : Model.GamesQuery) : Promise<Model.Game[]> {
        return await this.sendRequest<Model.GamesQuery, Model.Game[]>(
            HttpMethod.Post, "/games/query", query);
    }

    async removePlayer(gameId : number, playerId : number) : Promise<Model.StateAndEventResponse> {
        return await this.sendRequest<{}, Model.StateAndEventResponse>(
            HttpMethod.Delete, "/games/" + gameId + "/players/" + playerId);
    }

    async addPlayer(gameId : number, request : Model.CreatePlayerRequest) : Promise<Model.StateAndEventResponse> {
        return await this.sendRequest<Model.CreatePlayerRequest, Model.StateAndEventResponse>(
            HttpMethod.Post, "/games/" + gameId + "/players", request);
    }

    async startGame(gameId : number) : Promise<Model.StateAndEventResponse> {
        return await this.sendRequest<{}, Model.StateAndEventResponse>(
            HttpMethod.Post, "/games/" + gameId + "/start-request");
    }
}