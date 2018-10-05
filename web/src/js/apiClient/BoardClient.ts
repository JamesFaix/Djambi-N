import * as $ from 'jquery';
import {Board} from "./BoardModel";
import {Environment} from "./../Environment";

export class BoardClient {
    constructor() {

    }

    static async getBoard(regionCount : number) : Promise<Board> {
        let result : Board;

        await $.ajax({
            type: "GET",
            url: Environment.apiAddress() + "/boards/" + regionCount,
            dataType: "json",
            success: (data, status, xhr) => {
                result = data;
            },
            error: () => {

            }
        });

        return result;
    }

    static async getCellPaths(regionCount : number, cellId : number) : Promise<Array<Array<number>>> {
        let result: Array<Array<number>>;
        
        await $.ajax({
            type: "GET",
            url: Environment.apiAddress() + "/boards/" + regionCount + "/cells/" + cellId + "/paths",
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