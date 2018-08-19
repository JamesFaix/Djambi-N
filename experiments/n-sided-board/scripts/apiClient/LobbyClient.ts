/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import { LobbyGame } from "./LobbyModel.js";
import { GameStartResponse} from "./PlayModel.js";

export class LobbyClient {
    private static readonly baseUrl : string = "http://localhost:54835/api";
    constructor() {

    }

    static async createGame(regionCount : number) : Promise<LobbyGame> {
        let result : LobbyGame;

        await $.ajax({
            type: "POST",
            url: this.baseUrl + "/games",
            data: {
                boardRegionCount : regionCount
            },
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