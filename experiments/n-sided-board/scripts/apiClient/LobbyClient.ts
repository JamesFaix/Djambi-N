/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import { LobbyGame, GameCreationRequest } from "./LobbyModel.js";
import { GameStartResponse} from "./PlayModel.js";

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
}