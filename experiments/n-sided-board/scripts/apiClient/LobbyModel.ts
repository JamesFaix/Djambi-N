export class User {
    readonly id : number
    readonly name : string
}

export class LobbyPlayer {
    readonly id : number;
    readonly userId : number;
    readonly name : string;
}

export enum GameStatus {
    Open = "Open",
    Started = "Started",
    Complete = "Complete",
    Canceled = "Canceled"
}

export class LobbyGame {
    readonly id : number;
    readonly status : GameStatus;
    readonly boardRegionCount : number;
    readonly description : string
    readonly players : Array<LobbyPlayer>
}