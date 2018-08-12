/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import {Board} from "./model/Board.js";

export class BoardClient {
    constructor() {

    }

    static async getBoard(regionCount : number) : Promise<Board> {
        let result : Board;

        await $.ajax({
            type: "GET",
            url: "http://localhost:54835/api/boards/" + regionCount,
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
            url: "http://localhost:54835/api/boards/" + regionCount + "/cells/" + cellId + "/paths",
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