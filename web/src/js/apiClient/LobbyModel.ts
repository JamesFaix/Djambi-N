export enum Role {
    Admin = "Admin",
    Guest = "Guest",
    Normal = "Normal"
}

export class User {
    readonly id : number
    readonly name : string
    readonly role : Role
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

export class GameCreationRequest {
    constructor(
        readonly boardRegionCount : number,
        readonly descriptoin : string
    ){        
    }
}

export class CreateUserRequest {
    constructor(
        readonly name : string,
        readonly password : string,
        readonly role : Role
    ){
        
    }
}

export class SigninRequest {
    constructor(
        readonly username : string,
        readonly password : string
    ) {

    }
}