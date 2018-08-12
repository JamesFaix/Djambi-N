import {Cell} from "./Cell.js";

export class Board {
    constructor(
        readonly regionCount : number,
        readonly regionSize : number,
        readonly cells : Array<Cell>
    ) {

    }
}