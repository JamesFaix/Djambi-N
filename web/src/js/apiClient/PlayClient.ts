import * as $ from 'jquery';
import {CommitTurnResponse, TurnState} from "./PlayModel";

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

    static async commitTurn(gameId : number) : Promise<CommitTurnResponse> {
        let result : CommitTurnResponse;

        await $.ajax({
            type: "POST",
            url: this.baseUrl + "/games/" + gameId + "/current-turn/commit-request",
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }

    static async resetTurn(gameId : number) : Promise<TurnState> {
        let result : TurnState;

        await $.ajax({
            type: "POST",
            url: this.baseUrl + "/games/" + gameId + "/current-turn/reset-request",
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