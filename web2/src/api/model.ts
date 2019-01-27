/*
 * This file was generated with the Client Generator utility.
 * Do not manually edit.
 */
export interface Board {
	regionCount : number,
	regionSize : number,
	cells : Cell[],
}

export interface Cell {
	id : number,
	locations : Location[],
}

export interface CreatePlayerRequest {
	kind : PlayerKind,
	userId : number,
	name : string,
}

export interface CreateUserRequest {
	name : string,
	password : string,
}

export interface Session {
    user : User,
    id : number,
    token : string,
    expiresOn : Date,
    createdOn : Date
}

//Board
export interface Location {
    region : number,
    x : number,
    y : number
}

export interface DiffEffect<a> {
	kind : EffectKind,
	oldValue : a,
	newValue : a,
}

export interface DiffWithContextEffect<a, b> {
	kind : EffectKind,
	oldValue : a,
	newValue : a,
	context : b,
}

export type Effect =
	DiffEffect<GameStatus> |
	DiffEffect<number[]> |
	DiffEffect<GameParameters> |
	ScalarEffect<number> |
	ScalarEffect<number> |
	ScalarEffect<number[]> |
	ScalarEffect<number> |
	ScalarEffect<CreatePlayerRequest> |
	DiffWithContextEffect<number, number[]> |
	DiffWithContextEffect<number, number> |
	DiffEffect<Turn>

export enum EffectKind {
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
	CurrentTurnChanged = "CurrentTurnChanged",
}

export interface Event {
	id : number,
	createdByUserId : number,
	createdOn : Date,
	kind : EventKind,
	effects : Effect[],
}

export enum EventKind {
	GameParametersChanged = "GameParametersChanged",
	GameCanceled = "GameCanceled",
	PlayerJoined = "PlayerJoined",
	PlayerEjected = "PlayerEjected",
	PlayerQuit = "PlayerQuit",
	GameStarted = "GameStarted",
	TurnCommitted = "TurnCommitted",
	TurnReset = "TurnReset",
	CellSelected = "CellSelected",
}

export interface Game {
	id : number,
	createdOn : Date,
	createdByUserId : number,
	parameters : GameParameters,
	status : GameStatus,
	players : Player[],
	pieces : Piece[],
	turnCycle : number[],
	currentTurn : Turn,
}

export interface GameParameters {
	description : string,
	regionCount : number,
	isPublic : boolean,
	allowGuests : boolean,
}

export enum GameStatus {
	Pending = "Pending",
	AbortedWhilePending = "AbortedWhilePending",
	Started = "Started",
	Aborted = "Aborted",
	Finished = "Finished",
}

export interface GamesQuery {
	gameId : number,
	descriptionContains : string,
	createdByUserName : string,
	playerUserName : string,
	isPublic : boolean,
	allowGuests : boolean,
}

export interface Location {
	region : number,
	x : number,
	y : number,
}

export interface LoginRequest {
	username : string,
	password : string,
}

export interface Piece {
	id : number,
	kind : PieceKind,
	playerId : number,
	originalPlayerId : number,
	cellId : number,
}

export enum PieceKind {
	Chief = "Chief",
	Thug = "Thug",
	Reporter = "Reporter",
	Assassin = "Assassin",
	Diplomat = "Diplomat",
	Gravedigger = "Gravedigger",
	Corpse = "Corpse",
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
	startingTurnNumber : number,
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
}

export interface ScalarEffect<a> {
	kind : EffectKind,
	value : a,
}

export interface Selection {
	kind : SelectionKind,
	cellId : number,
	pieceId : number,
}

export enum SelectionKind {
	Subject = "Subject",
	Move = "Move",
	Target = "Target",
	Drop = "Drop",
	Vacate = "Vacate",
}

export interface SelectionRequest {
	cellId : number,
}

export interface StateAndEventResponse {
	game : Game,
	event : Event,
}

export interface Turn {
	status : TurnStatus,
	selections : Selection[],
	selectionOptions : number[],
	requiredSelectionKind : SelectionKind,
}

export enum TurnStatus {
	AwaitingSelection = "AwaitingSelection",
	AwaitingConfirmation = "AwaitingConfirmation",
}

export interface User {
	id : number,
	name : string,
	isAdmin : boolean,
}

