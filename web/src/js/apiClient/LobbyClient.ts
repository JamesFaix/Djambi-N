import * as $ from 'jquery';
import { LobbyGame, GameCreationRequest, User, CreateUserRequest, SigninRequest } from "./LobbyModel";
import { GameStartResponse} from "./PlayModel";
import {Environment} from "./../Environment";

export class LobbyClient {
    constructor() {

    }

    static async createGame(request : GameCreationRequest) : Promise<LobbyGame> {
        let result : LobbyGame;

        await $.ajax({
            type: "POST",
            url: Environment.apiAddress() + "/games",
            data: request,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });
        return result;
    }

    static async startGame(gameId : number) : Promise<GameStartResponse> {
        let result : GameStartResponse;

        await $.ajax({
            type: "POST",
            url: Environment.apiAddress() + "/games/" + gameId + "/start-request" ,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }
    
    static async getGames() : Promise<Array<LobbyGame>> {
        let result : Array<LobbyGame>;

        await $.ajax({
            type: "GET",
            url: Environment.apiAddress() + "/games" ,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }

    static async getUsers() : Promise<Array<User>> {
        let result : Array<User>;

        await $.ajax({
            type: "GET",
            url: Environment.apiAddress + "/users" ,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }

    static async createUser(request : CreateUserRequest) : Promise<User> {
        let result : User;

        await $.ajax({
            type: "POST",
            url: Environment.apiAddress + "/users" ,
            dataType: "json",
            data: request,
            success: (data, status, xhr) => {
                result = data;
            },
            error: (xhr, status, error) => {

            }
        });

        return result;        
    }

    static async signin(request : SigninRequest) : Promise<void> {
        $.ajaxSetup({
            crossDomain: true,
            xhrFields: {
                withCredentials: true
            }
        });
        
        await $.ajax({
            type: "POST",
            url: Environment.apiAddress + "/signin" ,
            dataType: "json",
            data: request,
            success: (data, status, xhr) => {
                console.log(data);
            },
            error: (xhr, status, error) => {

            }
        });      
    }

    static async signout() : Promise<void> {
        
        await $.ajax({
            type: "POST",
            url: Environment.apiAddress + "/signout" ,
            dataType: "json",
            success: (data, status, xhr) => {
                console.log(data);
            },
            error: (xhr, status, error) => {

            }
        });
    }
}