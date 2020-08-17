//-------- Error Handling --------

export interface ProblemDetails {
    status : number,
    title : string,
    type : string,
    traceId : string,
    detail : string,
    errors : object
}

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
    EditUsers = "EditUsers",
    EditPendingGames = "EditPendingGames",
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
    Pending = "Pending",
    InProgress = "InProgress",
    Canceled = "Canceled",
    Over = "Over",
}

export interface Piece {
    id : number,
    kind : PieceKind,
    playerId : number,
    originalPlayerId : number,
    cellId : number
}

export enum PieceKind {
    Conduit = "Conduit",
    Thug = "Thug",
    Scientist = "Scientist",
    Hunter = "Hunter",
    Diplomat = "Diplomat",
    Reaper = "Reaper",
    Corpse = "Corpse",
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
    User = "User",
    Guest = "Guest",
    Neutral = "Neutral",
}

export enum PlayerStatus {
    Pending = "Pending",
    Alive = "Alive",
    Eliminated = "Eliminated",
    Conceded = "Conceded",
    WillConcede = "WillConcede",
    AcceptsDraw = "AcceptsDraw",
    Victorious = "Victorious",
}

//-------- TURN --------

export interface Selection {
    kind : SelectionKind,
    cellId : number,
    pieceId : number
}

export enum SelectionKind {
    Subject = "Subject",
    Move = "Move",
    Target = "Target",
    Drop = "Drop",
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
    AwaitingSelection = "AwaitingSelection",
    AwaitingCommit = "AwaitingCommit",
    DeadEnd = "DeadEnd",
}

//-------- EVENTS --------

export interface Effect {
    kind : EffectKind
}

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
    GameParametersChanged = "GameParametersChanged",
    GameCanceled = "GameCanceled",
    PlayerJoined = "PlayerJoined",
    PlayerRemoved = "PlayerRemoved",
    GameStarted = "GameStarted",
    TurnCommitted = "TurnCommitted",
    TurnReset = "TurnReset",
    CellSelected = "CellSelected",
    PlayerStatusChanged = "PlayerStatusChanged",
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
    Ascending = "Ascending",
    Descending = "Descending",
}

