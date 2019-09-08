/*
 * This file was generated with the Client Generator utility.
 * Do not manually edit.
 */

//-------- USER --------

export interface CreateUserRequest {
    name : string,
    password : string
}

export interface CreationSource {
    userId : number,
    userName : string,
    time : Date
}

export enum Privilege {
    EditPendingGames = "EditPendingGames",
    EditUsers = "EditUsers",
    OpenParticipation = "OpenParticipation",
    Snapshots = "Snapshots",
    ViewGames = "ViewGames",
}

export interface User {
    id : number,
    name : string,
    privileges : Privilege[]
}

//-------- SESSION --------

export interface LoginRequest {
    username : string,
    password : string
}

export interface Session {
    id : number,
    user : User,
    token : string,
    createdOn : Date,
    expiresOn : Date
}

//-------- BOARD --------

export interface Board {
    regionCount : number,
    regionSize : number,
    cells : Cell[]
}

export interface Cell {
    id : number,
    locations : Location[]
}

export interface Location {
    region : number,
    x : number,
    y : number
}

//-------- GAME --------

export interface Game {
    id : number,
    createdBy : CreationSource,
    parameters : GameParameters,
    status : GameStatus,
    players : Player[],
    pieces : Piece[],
    turnCycle : number[],
    currentTurn : Turn
}

export interface GameParameters {
    description : string,
    regionCount : number,
    isPublic : boolean,
    allowGuests : boolean
}

export enum GameStatus {
    Canceled = "Canceled",
    InProgress = "InProgress",
    Over = "Over",
    Pending = "Pending",
}

export interface GamesQuery {
    gameId : number,
    descriptionContains : string,
    createdByUserName : string,
    playerUserName : string,
    isPublic : boolean,
    allowGuests : boolean,
    status : GameStatus
}

export interface Piece {
    id : number,
    kind : PieceKind,
    playerId : number,
    originalPlayerId : number,
    cellId : number
}

export enum PieceKind {
    Assassin = "Assassin",
    Chief = "Chief",
    Corpse = "Corpse",
    Diplomat = "Diplomat",
    Gravedigger = "Gravedigger",
    Reporter = "Reporter",
    Thug = "Thug",
}

//-------- PLAYER --------

export interface CreatePlayerRequest {
    kind : PlayerKind,
    userId : number,
    name : string
}

export interface Player {
    id : number,
    gameId : number,
    userId : number,
    kind : PlayerKind,
    name : string,
    status : PlayerStatus,
    colorId : number,
    startingRegion : number,
    startingTurnNumber : number
}

export enum PlayerKind {
    Guest = "Guest",
    Neutral = "Neutral",
    User = "User",
}

export enum PlayerStatus {
    AcceptsDraw = "AcceptsDraw",
    Alive = "Alive",
    Conceded = "Conceded",
    Eliminated = "Eliminated",
    Pending = "Pending",
    Victorious = "Victorious",
    WillConcede = "WillConcede",
}

//-------- TURN --------

export interface Selection {
    kind : SelectionKind,
    cellId : number,
    pieceId : number
}

export enum SelectionKind {
    Drop = "Drop",
    Move = "Move",
    Subject = "Subject",
    Target = "Target",
    Vacate = "Vacate",
}

export interface SelectionRequest {
    cellId : number
}

export interface Turn {
    status : TurnStatus,
    selections : Selection[],
    selectionOptions : number[],
    requiredSelectionKind : SelectionKind
}

export enum TurnStatus {
    AwaitingCommit = "AwaitingCommit",
    AwaitingSelection = "AwaitingSelection",
    DeadEnd = "DeadEnd",
}

//-------- EVENTS --------

export interface CurrentTurnChangedEffect {
    oldValue : Turn,
    newValue : Turn
}

export interface Effect {
    kind : EffectKind,
    value : EffectCase
}

export type EffectCase =
    CurrentTurnChangedEffect |
    GameStatusChangedEffect |
    NeutralPlayerAddedEffect |
    ParametersChangedEffect |
    PieceAbandonedEffect |
    PieceDroppedEffect |
    PieceEnlistedEffect |
    PieceKilledEffect |
    PieceMovedEffect |
    PieceVacatedEffect |
    PlayerAddedEffect |
    PlayerOutOfMovesEffect |
    PlayerRemovedEffect |
    PlayerStatusChangedEffect |
    TurnCycleAdvancedEffect |
    TurnCyclePlayerFellFromPowerEffect |
    TurnCyclePlayerRemovedEffect |
    TurnCyclePlayerRoseToPowerEffect

export enum EffectKind {
    CurrentTurnChanged = "CurrentTurnChanged",
    GameStatusChanged = "GameStatusChanged",
    NeutralPlayerAdded = "NeutralPlayerAdded",
    ParametersChanged = "ParametersChanged",
    PieceAbandoned = "PieceAbandoned",
    PieceDropped = "PieceDropped",
    PieceEnlisted = "PieceEnlisted",
    PieceKilled = "PieceKilled",
    PieceMoved = "PieceMoved",
    PieceVacated = "PieceVacated",
    PlayerAdded = "PlayerAdded",
    PlayerOutOfMoves = "PlayerOutOfMoves",
    PlayerRemoved = "PlayerRemoved",
    PlayerStatusChanged = "PlayerStatusChanged",
    TurnCycleAdvanced = "TurnCycleAdvanced",
    TurnCyclePlayerFellFromPower = "TurnCyclePlayerFellFromPower",
    TurnCyclePlayerRemoved = "TurnCyclePlayerRemoved",
    TurnCyclePlayerRoseToPower = "TurnCyclePlayerRoseToPower",
}


export interface Event {
    id : number,
    createdBy : CreationSource,
    actingPlayerId : number,
    kind : EventKind,
    effects : Effect[]
}

export enum EventKind {
    CellSelected = "CellSelected",
    CorrectNeutralPiecePlayerIds = "CorrectNeutralPiecePlayerIds",
    GameCanceled = "GameCanceled",
    GameParametersChanged = "GameParametersChanged",
    GameStarted = "GameStarted",
    PlayerJoined = "PlayerJoined",
    PlayerRemoved = "PlayerRemoved",
    PlayerStatusChanged = "PlayerStatusChanged",
    TurnCommitted = "TurnCommitted",
    TurnReset = "TurnReset",
}

export interface EventsQuery {
    maxResults : number,
    direction : ResultsDirection,
    thresholdTime : Date,
    thresholdEventId : number
}

export interface GameStatusChangedEffect {
    oldValue : GameStatus,
    newValue : GameStatus
}

export interface NeutralPlayerAddedEffect {
    name : string,
    placeholderPlayerId : number
}

export interface ParametersChangedEffect {
    oldValue : GameParameters,
    newValue : GameParameters
}

export interface PieceAbandonedEffect {
    oldPiece : Piece
}

export interface PieceDroppedEffect {
    oldPiece : Piece,
    newPiece : Piece
}

export interface PieceEnlistedEffect {
    oldPiece : Piece,
    newPlayerId : number
}

export interface PieceKilledEffect {
    oldPiece : Piece
}

export interface PieceMovedEffect {
    oldPiece : Piece,
    newCellId : number
}

export interface PieceVacatedEffect {
    oldPiece : Piece,
    newCellId : number
}

export interface PlayerAddedEffect {
    name : string,
    userId : number,
    kind : PlayerKind
}

export interface PlayerOutOfMovesEffect {
    playerId : number
}

export interface PlayerRemovedEffect {
    playerId : number
}

export interface PlayerStatusChangedEffect {
    playerId : number,
    oldStatus : PlayerStatus,
    newStatus : PlayerStatus
}

export interface StateAndEventResponse {
    game : Game,
    event : Event
}

export interface TurnCycleAdvancedEffect {
    oldValue : number[],
    newValue : number[]
}

export interface TurnCyclePlayerFellFromPowerEffect {
    oldValue : number[],
    newValue : number[],
    playerId : number
}

export interface TurnCyclePlayerRemovedEffect {
    oldValue : number[],
    newValue : number[],
    playerId : number
}

export interface TurnCyclePlayerRoseToPowerEffect {
    oldValue : number[],
    newValue : number[],
    playerId : number
}

//-------- SNAPSHOTS --------

export interface CreateSnapshotRequest {
    description : string
}

export interface SnapshotInfo {
    id : number,
    createdBy : CreationSource,
    description : string
}

//-------- MISC --------

export enum ResultsDirection {
    Ascending = "Ascending",
    Descending = "Descending",
}

