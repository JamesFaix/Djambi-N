/// <reference path ="../../node_modules/definitely-typed-jquery/jquery.d.ts"/> 

import {Location, Cell, Board} from "./BoardModel.js";

export class BoardClient {
    private static readonly baseUrl : string = "http://localhost:54835/api";
    constructor() {

    }

    static async getBoard(regionCount : number) : Promise<Board> {
        let result : Board;

        await $.ajax({
            type: "GET",
            url: this.baseUrl + "/boards/" + regionCount,
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
            url: this.baseUrl + "/boards/" + regionCount + "/cells/" + cellId + "/paths",
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