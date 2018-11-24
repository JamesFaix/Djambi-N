export class CreateUserRequest {
    constructor(
        public readonly name : string,
        public readonly password : string
    ){}
}

export class UserResponse {
    constructor(
        public readonly id : number,
        public readonly name : string,
        public readonly isAdmin : boolean
    ){}
}

export class LoginRequest {
    constructor(
        public readonly username : string,
        public readonly password : string
    ){}
}

export class CreateLobbyRequest {
    constructor(
        public readonly regionCount : number,
        public readonly description: string,
        public readonly allowGuests : boolean,
        public readonly isPublic : boolean
    ){}
}

export class LobbyResponse {
    constructor(
        public readonly id : number,
        public readonly regionCount : number,
        public readonly description : string,
        public readonly allowGuests : boolean,
        public readonly isPublic : boolean
    ){}
}

export enum PlayerType {
    User = "user",
    Guest = "guest",
    Virtual = "virtual"
}

export class CreatePlayerRequest {
    constructor(
        public readonly userId : number,
        public readonly name : string,
        public readonly type : PlayerType
    ){}
}

export class PlayerResponse {
    constructor(
        public readonly id : number,
        public readonly userId : number,
        public readonly name : string,
        public readonly type : PlayerType
    ){}
}

export class LobbyWithPlayersResponse {
    constructor(
        public readonly id : number,
        public readonly regionCount : number,
        public readonly description : string,
        public readonly allowGuests : boolean,
        public readonly isPublic : boolean,
        public readonly players : PlayerResponse[]
    ){}
}

export class LobbiesQueryRequest {
    constructor(
        public readonly descriptionContains : string,
        public readonly createdByUserId : number,
        public readonly playerUserId : number,
        public readonly isPublic : boolean,
        public readonly allowGuests : boolean
    ){}
}

export class LocationResponse {
    constructor(
        public readonly region : number,
        public readonly x : number,
        public readonly y : number
    ){}
}

export class CellResponse {
    constructor(
        public readonly id : number,
        public readonly locations : LocationResponse[]
    ){}
}