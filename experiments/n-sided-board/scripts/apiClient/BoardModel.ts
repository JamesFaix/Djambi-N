export class Location {
    constructor(
        readonly x: number,
        readonly y: number,
        readonly region: number) {
    }
}

export class Cell {
    constructor (
        readonly id : number,
        readonly locations : Array<Location>){
    }
}

export class Board {
    constructor(
        readonly regionCount : number,
        readonly regionSize : number,
        readonly cells : Array<Cell>) {        
    }
}