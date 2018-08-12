import {Location} from "./Location.js";

export class Cell {
    constructor(
        readonly id : number,
        readonly locations : Array<Location>
    ){

    }
}