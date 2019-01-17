//Users
export interface User {
    id : number,
    name : string,
    isAdmin : boolean
}

export interface CreateUserRequest {
    name : string,
    password : string
}

//Session
export interface LoginRequest {
    username : string,
    password : string
}

//Board
export interface Location {
    region : number,
    x : number,
    y : number
}

export interface Cell {
    id : number,
    locations : Location[]
}

export interface Board {
    regionCount : number,
    regionSize : number,
    cells : Cell[]
}

//Players
export enum PlayerKind {
    User = "User",
    Guest = "Guest",
    Neutral = "Neutral"
}

export interface Player {
    id : number,
    gameId : number,
    userId : number,
    kind : PlayerKind,
    name : string,
    isAlive : boolean,
    colorId : number,
    startingRegion : number,
    startingTurnNumber : number
}

export interface CreatePlayerRequest {
    userId : number,
    name : string,
    type : PlayerKind
}

//Pieces
export enum PieceKind {
    Chief = "Chief",
    Thug = "Thug",
    Reporter = "Reporter",
    Assassin = "Assassin",
    Diplomat = "Diplomat",
    Gravedigger = "Gravedigger",
    Corpse = "Corpse"
}

export interface Piece {
    id : number,
    kind : PieceKind,
    playerId : number,
    originalPlayerId : number,
    cellId : number
}

//Selections
export enum SelectionKind {
    Subject = "Subject",
    Move = "Move",
    Target = "Target",
    Drop = "Drop",
    Vacate = "Vacate"
}

export interface Selection {
    kind : SelectionKind,
    cellId : number,
    pieceId : number
}

export interface SelectionRequest {
    cellId : number
}

//Turns
export enum TurnStatus {
    AwaitingConfirmation = "AwaitingConfirmation",
    AwaitingSelection = "AwaitingSelection"
}

export interface Turn {
    status : TurnStatus,
    selections : Selection[],
    selectionOptions : number[],
    requiredSelectionKind : SelectionKind
}

//Game
export enum GameStatus {
    Pending = "Pending",
    AbortedWhilePending = "AbortedWhilePending",
    Started = "Started",
    Aborted = "Aborted",
    Finished = "Finished"
}

export interface GameParameters {
    regionCount : number,
    description: string,
    allowGuests : boolean,
    isPublic : boolean
}

export interface Game {
    id : number,
    createdByUserId : number,
    createdOn : Date,
    parameters : GameParameters,
    status : GameStatus,
    players : Player[],
    pieces : Piece[],
    turnCycle : number[],
    currentTurn : Turn
}

export interface GamesQuery {
    gameId : number,
    descriptionContains : string,
    createdByUserId : number,
    playerUserId : number,
    isPublic : boolean,
    allowGuests : boolean
}

export interface CreateGameRequest {
    parameters : GameParameters,
    createdByUserId : number
}

export enum EffectKind {
    GameCreated = "GameCreated",
    GameStatusChanged = "GameStatusChanged",
    PlayerEliminated = "PlayerEliminated",
    PiecesOwnershipChanged = "PiecesOwnershipChanged",
    PieceMoved = "PieceMoved",
    PieceKilled = "PieceKilled",
    TurnCycleChanged = "TurnCycleChanged",
    PlayerOutOfMoves = "PlayerOutOfMoves",
    PlayerAdded = "PlayerAdded",
    PlayersRemoved = "PlayersRemoved",
    ParametersChanged = "ParametersChanged",
    CurrentTurnChanged = "CurrentTurnChanged"
}

export type DiffEffect<T> = {
    kind : EffectKind,
    oldValue : T,
    newValue : T
}

export type ScalarEffect<T> = {
    kind : EffectKind,
    value : T
}

export type DiffWithContextEffect<T, U> = {
    kind : EffectKind,
    oldValue : T,
    newValue : T,
    context : U
}

export type Effect =
    ScalarEffect<CreateGameRequest> |
    DiffEffect<GameStatus> |
    DiffEffect<number[]> |
    DiffEffect<GameParameters> |
    ScalarEffect<number> |
    ScalarEffect<number[]> |
    ScalarEffect<CreatePlayerRequest> |
    DiffWithContextEffect<number, number[]> |
    DiffWithContextEffect<number, number> |
    DiffEffect<Turn>

export enum EventKind {
    GameCreated = "GameCreated",
    GameParametersChanged = "GameParametersChanged",
    GameCanceled = "GameCanceled",
    PlayerJoined = "PlayerJoined",
    PlayerEjected = "PlayerEjected",
    PlayerQuit = "PlayerQuit",
    GameStarted = "GameStarted",
    TurnCommitted = "TurnCommitted",
    TurnReset = "TurnReset",
    CellSelected = "CellSelected"
}

export interface Event {
    kind : EventKind,
    timestamp : Date,
    effects : Effect[]
}

export interface StateAndEventResponse {
    game : Game,
    event : Event
}