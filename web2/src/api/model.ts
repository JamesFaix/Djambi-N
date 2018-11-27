export interface CreateUserRequest {
    name : string,
    password : string
}

export interface UserResponse {
    id : number,
    name : string,
    isAdmin : boolean
}

export interface LoginRequest {
    username : string,
    password : string
}

export interface CreateLobbyRequest {
    regionCount : number,
    description: string,
    allowGuests : boolean,
    isPublic : boolean
}

export interface LobbyResponse {
    id : number,
    regionCount : number,
    description : string,
    allowGuests : boolean,
    isPublic : boolean,
    createdByUserId : number,
    createdOn : Date
}

export enum PlayerType {
    User = "User",
    Guest = "Guest",
    Virtual = "Virtual"
}

export interface CreatePlayerRequest {
    userId : number,
    name : string,
    type : PlayerType
}

export interface PlayerResponse {
    id : number,
    userId : number,
    name : string,
    type : PlayerType
}

export interface LobbyWithPlayersResponse {
    id : number,
    regionCount : number,
    description : string,
    allowGuests : boolean,
    isPublic : boolean,
    createdByUserId : number,
    createdOn : Date,
    players : PlayerResponse[]
}

export interface LobbiesQueryRequest {
    descriptionContains : string,
    createdByUserId : number,
    playerUserId : number,
    isPublic : boolean,
    allowGuests : boolean
}

export interface LocationResponse {
    region : number,
    x : number,
    y : number
}

export interface CellResponse {
    id : number,
    locations : LocationResponse[]
}