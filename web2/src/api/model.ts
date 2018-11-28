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

export interface CreateSelectionRequest {
    cellId : number
}

export interface PlayerStateResponse {
    id : number,
    isAlive : boolean
}

export enum PieceType {
    Chief = "Chief",
    Thug = "Thug",
    Reporter = "Reporter",
    Assassin = "Assassin",
    Diplomat = "Diplomat",
    Gravedigger = "Gravedigger",
    Corpse = "Corpse"
}

export interface PieceResponse {
    id : number,
    type : PieceType,
    playerId : number,
    originalPlayerId : number,
    cellId : number
}

export interface GameStateResponse {
    players : PlayerStateResponse[],
    pieces : PieceResponse[],
    turnCycle : number[]
}

export interface PlayerStartConditionsResponse {
    playerId : number,
    turNumber : number,
    region : number,
    color : number
}

export enum SelectionType {
    Subject = "Subject",
    Move = "Move",
    Target = "Target",
    Drop = "Drop",
    Vacate = "Vacate"
}

export interface SelectionResponse {
    type : SelectionType,
    cellId : number,
    pieceId : number
}

export enum TurnStatus {
    AwaitingConfirmation = "AwaitingConfirmation",
    AwaitingSelection = "AwaitingSelection"
}

export interface TurnStateResponse {
    status : TurnStatus,
    selections : SelectionResponse[],
    selectionOptions : number[],
    requiredSelectionType : SelectionType
}

export interface GameStartResponse {
    gameState : GameStateResponse,
    startingConditions : PlayerStartConditionsResponse[],
    turnState : TurnStateResponse
}

export interface CommitTurnResponse {
    gameState : GameStateResponse,
    turnState : TurnStateResponse
}