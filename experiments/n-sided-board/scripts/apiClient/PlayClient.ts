/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import {TurnState} from "./PlayModel.js";

export class PlayClient {
    private static readonly baseUrl : string = "http://localhost:54835/api";
    constructor() {

    }

    static async selectCell(gameId : number, cellId : number) : Promise<TurnState> {
        let result : TurnState;

        await $.ajax({
            type: "POST",
            url: this.baseUrl + "/games/" + gameId + "/current-turn/selection-request/" + cellId,
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