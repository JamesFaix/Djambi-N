/*
 * This file was generated with the Client Generator utility.
 * Do not manually edit.
 */

//-------- USER --------

export interface CreateUserRequest {
	name : string,
	password : string,
}

export interface User {
	id : number,
	name : string,
	isAdmin : boolean,
}

//-------- SESSION --------

export interface LoginRequest {
	username : string,
	password : string,
}

export interface Session {
	id : number,
	user : User,
	token : string,
	createdOn : Date,
	expiresOn : Date,
}

//-------- BOARD --------

export interface Board {
	regionCount : number,
	regionSize : number,
	cells : Cell[],
}

export interface Cell {
	id : number,
	locations : Location[],
}

export interface Location {
	region : number,
	x : number,
	y : number,
}

//-------- GAME --------

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
	Aborted = "Aborted",
	AbortedWhilePending = "AbortedWhilePending",
	Finished = "Finished",
	Pending = "Pending",
	Started = "Started",
}

export interface GamesQuery {
	gameId : number,
	descriptionContains : string,
	createdByUserName : string,
	playerUserName : string,
	isPublic : boolean,
	allowGuests : boolean,
}

export interface Piece {
	id : number,
	kind : PieceKind,
	playerId : number,
	originalPlayerId : number,
	cellId : number,
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
	name : string,
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
	Guest = "Guest",
	Neutral = "Neutral",
	User = "User",
}

export enum PlayerStatus {
	Alive = "Alive",
	Eliminated = "Eliminated",
	Pending = "Pending",
}

//-------- TURN --------

export interface Selection {
	kind : SelectionKind,
	cellId : number,
	pieceId : number,
}

export enum SelectionKind {
	Drop = "Drop",
	Move = "Move",
	Subject = "Subject",
	Target = "Target",
	Vacate = "Vacate",
}

export interface SelectionRequest {
	cellId : number,
}

export interface Turn {
	status : TurnStatus,
	selections : Selection[],
	selectionOptions : number[],
	requiredSelectionKind : SelectionKind,
}

export enum TurnStatus {
	AwaitingConfirmation = "AwaitingConfirmation",
	AwaitingSelection = "AwaitingSelection",
}

//-------- EVENTS --------

export interface CurrentTurnChangedEffect {
	oldValue : Turn,
	newValue : Turn,
}

export interface Effect {
	kind : EffectKind,
	value : EffectCase,
}

export type EffectCase =
	GameStatusChangedEffect |
	TurnCycleChangedEffect |
	ParametersChangedEffect |
	PlayerEliminatedEffect |
	PieceKilledEffect |
	PlayersRemovedEffect |
	PlayerOutOfMovesEffect |
	PlayerAddedEffect |
	PiecesOwnershipChangedEffect |
	PieceMovedEffect |
	CurrentTurnChangedEffect

export enum EffectKind {
	CurrentTurnChanged = "CurrentTurnChanged",
	GameStatusChanged = "GameStatusChanged",
	ParametersChanged = "ParametersChanged",
	PieceKilled = "PieceKilled",
	PieceMoved = "PieceMoved",
	PiecesOwnershipChanged = "PiecesOwnershipChanged",
	PlayerAdded = "PlayerAdded",
	PlayerEliminated = "PlayerEliminated",
	PlayerOutOfMoves = "PlayerOutOfMoves",
	PlayersRemoved = "PlayersRemoved",
	TurnCycleChanged = "TurnCycleChanged",
}


export interface Event {
	id : number,
	createdByUserId : number,
	createdOn : Date,
	kind : EventKind,
	effects : Effect[],
}

export enum EventKind {
	CellSelected = "CellSelected",
	GameCanceled = "GameCanceled",
	GameParametersChanged = "GameParametersChanged",
	GameStarted = "GameStarted",
	PlayerEjected = "PlayerEjected",
	PlayerJoined = "PlayerJoined",
	PlayerQuit = "PlayerQuit",
	TurnCommitted = "TurnCommitted",
	TurnReset = "TurnReset",
}

export interface EventsQuery {
	maxResults : number,
	direction : ResultsDirection,
	thresholdTime : Date,
	thresholdEventId : number,
}

export interface GameStatusChangedEffect {
	oldValue : GameStatus,
	newValue : GameStatus,
}

export interface ParametersChangedEffect {
	oldValue : GameParameters,
	newValue : GameParameters,
}

export interface PieceKilledEffect {
	pieceId : number,
}

export interface PieceMovedEffect {
	oldCellId : number,
	newCellId : number,
	pieceId : number,
}

export interface PiecesOwnershipChangedEffect {
	oldPlayerId : number,
	newPlayerId : number,
	pieceIds : number[],
}

export interface PlayerAddedEffect {
	name : string,
	userId : number,
	kind : PlayerKind,
}

export interface PlayerEliminatedEffect {
	playerId : number,
}

export interface PlayerOutOfMovesEffect {
	playerId : number,
}

export interface PlayersRemovedEffect {
	playerIds : number[],
}

export interface StateAndEventResponse {
	game : Game,
	event : Event,
}

export interface TurnCycleChangedEffect {
	oldValue : number[],
	newValue : number[],
}

//-------- MISC --------

export enum ResultsDirection {
	Ascending = "Ascending",
	Descending = "Descending",
}

