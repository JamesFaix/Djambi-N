import * as Model from './model';
import * as $ from 'jquery';
import Environment from "../environment";

export default class ApiClient {

    async getCurrentUser() : Promise<Model.UserResponse> {
        let result : Model.UserResponse;
        return await $.ajax({
            type : "GET",
            url: Environment.apiAddress() + "/users/current",
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Get current user succeeded");
                result = data;
            },
            error : () => {
                console.log("Get current user failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async createUser(request : Model.CreateUserRequest) : Promise<Model.UserResponse> {
        let result : Model.UserResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/users",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create user succeeded");
                result = data;
            },
            error : () => {
                console.log("Create user failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async login(request : Model.LoginRequest) : Promise<Model.UserResponse> {
        let result : Model.UserResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/sessions",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create session succeeded");
                result = data;
            },
            error : () => {
                console.log("Create session failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async logout() : Promise<void> {
        return await $.ajax({
            type : "DELETE",
            url: Environment.apiAddress() + "/sessions",
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Close session succeeded");
            },
            error : x => {
                console.log(x);
                console.log("Close session failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        });
    }

    async createLobby(request : Model.CreateLobbyRequest) : Promise<Model.LobbyResponse> {
        let result : Model.LobbyResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/lobbies",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create lobby succeeded");
                result = data;
            },
            error : () => {
                console.log("Create lobby failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async getLobby(lobbyId : number) : Promise<Model.LobbyWithPlayersResponse> {
        let result : Model.LobbyWithPlayersResponse;

        return await $.ajax({
            type : "GET",
            url: Environment.apiAddress() + "/lobbies/" + lobbyId,
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Get lobby succeeded");
                result = data;
            },
            error : () => {
                console.log("Get lobby failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async getLobbies(query : Model.LobbiesQueryRequest) : Promise<Model.LobbyResponse[]> {
        let result : Model.LobbyResponse[];

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/lobbies/query",
            data: query,
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Get lobbies succeeded");
                result = data;
            },
            error : () => {
                console.log("Get lobbies failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async removePlayer(lobbyId : number, playerId : number) : Promise<void> {
        return await $.ajax({
            type : "DELETE",
            url: Environment.apiAddress() + "/lobbies/" + lobbyId + "/players/" + playerId,
            success: (data, status, xhr) => {
                console.log("Remove player succeeded");
            },
            error : () => {
                console.log("Remove player failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
    }

    async addPlayer(lobbyId : number, request : Model.CreatePlayerRequest) : Promise<Model.PlayerResponse> {
        let result : Model.PlayerResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/lobbies/" + lobbyId + "/players",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Add player succeeded");
                result = data;
            },
            error : () => {
                console.log("Add player failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async startGame(lobbyId : number) : Promise<Model.GameStartResponse> {
        let result : Model.GameStartResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/lobbies/" + lobbyId + "/start-request",
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Start game succeeded");
                result = data;
            },
            error : () => {
                console.log("Start game failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }
}