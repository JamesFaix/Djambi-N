import * as Model from './model';
import * as $ from 'jquery';
import Environment from "../environment";
import { any } from 'prop-types';

enum HttpMethod {
    Get,
    Put,
    Post,
    Delete
}

export default class ApiClient {

    private async sendRequest<TBody, TResponse>(
        method : HttpMethod,
        route : string,
        body : TBody = null)
        : Promise<TResponse> {

        const endpoint = method.toString().toUpperCase() + route
        let result : TResponse;

        const ajaxSettings : JQuery.AjaxSettings<any> = {
            type : method.toString(),
            url: Environment.apiAddress() + route,
            dataType : "json",
            success: (data : TResponse, status : JQuery.Ajax.SuccessTextStatus, xhr : JQuery.jqXHR<any>) => {
                console.log(endpoint + " succeeded");
                result = data;
            },
            error : () => {
                console.log(endpoint + " failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        }

        if (body) {
           ajaxSettings.data = body;
        }

        return await $.ajax(ajaxSettings)
            .then(_ => result);
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
    async createGame(request : Model.GameParameters) : Promise<Model.StateAndEventResponse> {
        return await this.sendRequest<Model.GameParameters, Model.StateAndEventResponse>(
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