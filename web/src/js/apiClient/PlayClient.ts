import * as $ from 'jquery';
import {CommitTurnResponse, TurnState} from "./PlayModel";
import {Environment} from "./../Environment";

export class PlayClient {
    constructor() {

    }

    static async selectCell(gameId : number, cellId : number) : Promise<TurnState> {
        let result : TurnState;

        await $.ajax({
            type: "POST",
            url: Environment.apiAddress() + "/games/" + gameId + "/current-turn/selection-request/" + cellId,
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
            url: Environment.apiAddress() + "/games/" + gameId + "/current-turn/commit-request",
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
            url: Environment.apiAddress() + "/games/" + gameId + "/current-turn/reset-request",
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