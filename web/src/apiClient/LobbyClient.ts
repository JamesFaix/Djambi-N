import * as $ from 'jquery';
import { LobbyGame, GameCreationRequest, User } from "./LobbyModel";
import { GameStartResponse} from "./PlayModel";

export class LobbyClient {
    private static readonly baseUrl : string = "http://localhost:54835/api";
    constructor() {

    }

    static async createGame(request : GameCreationRequest) : Promise<LobbyGame> {
        let result : LobbyGame;

        await $.ajax({
            type: "POST",
            url: this.baseUrl + "/games",
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
            url: this.baseUrl + "/games/" + gameId + "/start-request" ,
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
            url: this.baseUrl + "/games" ,
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
            url: this.baseUrl + "/users" ,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }
}