/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import {GameStatus, LobbyGame, LobbyPlayer, User} from "./LobbyModel.js";

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
}