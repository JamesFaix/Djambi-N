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
    EditUsers = 1,
    EditPendingGames = 2,
    OpenParticipation = 3,
    Snapshots = 4,
    ViewGames = 5,
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
    Pending = 1,
    InProgress = 2,
    Canceled = 3,
    Over = 4,
}

export interface Piece {
    id : number,
    kind : PieceKind,
    playerId : number,
    originalPlayerId : number,
    cellId : number
}

export enum PieceKind {
    Conduit = 1,
    Thug = 2,
    Scientist = 3,
    Hunter = 4,
    Diplomat = 5,
    Reaper = 6,
    Corpse = 7,
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
    User = 1,
    Guest = 2,
    Neutral = 3,
}

export enum PlayerStatus {
    Pending = 1,
    Alive = 2,
    Eliminated = 3,
    Conceded = 4,
    WillConcede = 5,
    AcceptsDraw = 6,
    Victorious = 7,
}

//-------- TURN --------

export interface Selection {
    kind : SelectionKind,
    cellId : number,
    pieceId : number
}

export enum SelectionKind {
    Subject = 1,
    Move = 2,
    Target = 3,
    Drop = 4,
    Vacate = 5,
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
    AwaitingSelection = 1,
    AwaitingCommit = 2,
    DeadEnd = 3,
}

//-------- EVENTS --------

export interface Effect {
    kind : EffectKind
}

export enum EffectKind {
    CurrentTurnChanged = 1,
    GameStatusChanged = 2,
    NeutralPlayerAdded = 3,
    ParametersChanged = 4,
    PieceAbandoned = 5,
    PieceDropped = 6,
    PieceEnlisted = 7,
    PieceKilled = 8,
    PieceMoved = 9,
    PieceVacated = 10,
    PlayerAdded = 11,
    PlayerOutOfMoves = 12,
    PlayerRemoved = 13,
    PlayerStatusChanged = 14,
    TurnCycleAdvanced = 15,
    TurnCyclePlayerFellFromPower = 16,
    TurnCyclePlayerRemoved = 17,
    TurnCyclePlayerRoseToPower = 18,
}

export interface Event {
    id : number,
    createdBy : CreationSource,
    actingPlayerId : number,
    kind : EventKind,
    effects : Effect[]
}

export enum EventKind {
    GameParametersChanged = 1,
    GameCanceled = 2,
    PlayerJoined = 3,
    PlayerRemoved = 4,
    GameStarted = 5,
    TurnCommitted = 6,
    TurnReset = 7,
    CellSelected = 8,
    PlayerStatusChanged = 9,
}

export interface EventsQuery {
    maxResults : number,
    direction : ResultsDirection,
    thresholdTime : Date,
    thresholdEventId : number
}

export interface CurrentTurnChangedEffect extends Effect {
    oldValue : Turn,
    newValue : Turn
}

export interface GameStatusChangedEffect extends Effect {
    oldValue : GameStatus,
    newValue : GameStatus
}

export interface NeutralPlayerAddedEffect extends Effect {
    name : string,
    placeholderPlayerId : number
}

export interface ParametersChangedEffect extends Effect {
    oldValue : GameParameters,
    newValue : GameParameters
}

export interface PieceAbandonedEffect extends Effect {
    oldPiece : Piece
}

export interface PieceDroppedEffect extends Effect {
    oldValue : Piece,
    newValue : Piece
}

export interface PieceEnlistedEffect extends Effect {
    oldPiece : Piece,
    newPlayerId : number
}

export interface PieceKilledEffect extends Effect {
    oldPiece : Piece
}

export interface PieceMovedEffect extends Effect {
    oldPiece : Piece,
    newCellId : number
}

export interface PieceVacatedEffect extends Effect {
    oldPiece : Piece,
    newCellId : number
}

export interface PlayerAddedEffect extends Effect {
    name : string,
    userId : number,
    playerKind : PlayerKind
}

export interface PlayerOutOfMovesEffect extends Effect {
    playerId : number
}

export interface PlayerRemovedEffect extends Effect {
    oldPlayer : Player
}

export interface PlayerStatusChangedEffect extends Effect {
    playerId : number,
    oldValue : PlayerStatus,
    newValue : PlayerStatus
}

export interface StateAndEventResponse {
    game : Game,
    event : Event
}

export interface TurnCycleAdvancedEffect extends Effect {
    oldValue : number[],
    newValue : number[]
}

export interface TurnCyclePlayerFellFromPowerEffect extends Effect {
    oldValue : number[],
    newValue : number[],
    playerId : number
}

export interface TurnCyclePlayerRemovedEffect extends Effect {
    oldValue : number[],
    newValue : number[],
    playerId : number
}

export interface TurnCyclePlayerRoseToPowerEffect extends Effect {
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

//-------- SEARCH --------

export interface GamesQuery {
    gameId : number,
    descriptionContains : string,
    createdByUserName : string,
    playerUserName : string,
    containsMe : boolean,
    isPublic : boolean,
    allowGuests : boolean,
    statuses : GameStatus[],
    createdBefore : Date,
    createdAfter : Date,
    lastEventBefore : Date,
    lastEventAfter : Date
}

export interface SearchGame {
    id : number,
    parameters : GameParameters,
    createdBy : CreationSource,
    status : GameStatus,
    lastEventOn : Date,
    playerCount : number,
    containsMe : boolean
}

//-------- MISC --------

export enum ResultsDirection {
    Ascending = 0,
    Descending = 1,
}

