/*
 * This file was generated with the Client Generator utility.
 * Do not manually edit.
 */

import * as Model from './model';
import {ApiClientCore, HttpMethod} from './clientCore';

export default class ApiClient {

//-------- USER --------

	async createUser(request : Model.CreateUserRequest) : Promise<Model.User> {
		const route = "/users";
		return await ApiClientCore.sendRequest<Model.CreateUserRequest, Model.User>(
			HttpMethod.Post, route, request);
	}

	async deleteUser(userId : number) : Promise<{}> {
		const route = "/users/" + userId + "";
		return await ApiClientCore.sendRequest<{}, {}>(
			HttpMethod.Delete, route);
	}

	async getCurrentUser() : Promise<Model.User> {
		const route = "/users/current";
		return await ApiClientCore.sendRequest<{}, Model.User>(
			HttpMethod.Get, route);
	}

	async getUser(userId : number) : Promise<Model.User> {
		const route = "/users/" + userId + "";
		return await ApiClientCore.sendRequest<{}, Model.User>(
			HttpMethod.Get, route);
	}

//-------- SESSION --------

	async login(request : Model.LoginRequest) : Promise<Model.Session> {
		const route = "/sessions";
		return await ApiClientCore.sendRequest<Model.LoginRequest, Model.Session>(
			HttpMethod.Post, route, request);
	}

	async logout() : Promise<{}> {
		const route = "/sessions";
		return await ApiClientCore.sendRequest<{}, {}>(
			HttpMethod.Delete, route);
	}

//-------- GAME --------

	async createGame(parameters : Model.GameParameters) : Promise<Model.Game> {
		const route = "/games";
		return await ApiClientCore.sendRequest<Model.GameParameters, Model.Game>(
			HttpMethod.Post, route, parameters);
	}

	async getGame(gameId : number) : Promise<Model.Game> {
		const route = "/games/" + gameId + "";
		return await ApiClientCore.sendRequest<{}, Model.Game>(
			HttpMethod.Get, route);
	}

	async getGames(query : Model.GamesQuery) : Promise<Model.Game[]> {
		const route = "/games/query";
		return await ApiClientCore.sendRequest<Model.GamesQuery, Model.Game[]>(
			HttpMethod.Post, route, query);
	}

	async startGame(gameId : number) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/start-request";
		return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
			HttpMethod.Post, route);
	}

	async updateGameParameters(gameId : number, parameters : Model.GameParameters) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/parameters";
		return await ApiClientCore.sendRequest<Model.GameParameters, Model.StateAndEventResponse>(
			HttpMethod.Put, route, parameters);
	}

//-------- EVENTS --------

	async getEvents(gameId : number, query : Model.EventsQuery) : Promise<Model.Event[]> {
		const route = "/games/" + gameId + "/events/query";
		return await ApiClientCore.sendRequest<Model.EventsQuery, Model.Event[]>(
			HttpMethod.Post, route, query);
	}

//-------- PLAYER --------

	async addPlayer(gameId : number, request : Model.CreatePlayerRequest) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/players";
		return await ApiClientCore.sendRequest<Model.CreatePlayerRequest, Model.StateAndEventResponse>(
			HttpMethod.Post, route, request);
	}

	async removePlayer(gameId : number, playerId : number) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/players/" + playerId + "";
		return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
			HttpMethod.Delete, route);
	}

//-------- TURN --------

	async commitTurn(gameId : number) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/current-turn/commit-request";
		return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
			HttpMethod.Post, route);
	}

	async resetTurn(gameId : number) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/current-turn/reset-request";
		return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
			HttpMethod.Post, route);
	}

	async selectCell(gameId : number, cellId : number) : Promise<Model.StateAndEventResponse> {
		const route = "/games/" + gameId + "/current-turn/selection-request/" + cellId + "";
		return await ApiClientCore.sendRequest<{}, Model.StateAndEventResponse>(
			HttpMethod.Post, route);
	}

//-------- BOARD --------

	async getBoard(regionCount : number) : Promise<Model.Board> {
		const route = "/boards/" + regionCount + "";
		return await ApiClientCore.sendRequest<{}, Model.Board>(
			HttpMethod.Get, route);
	}

	async getCellPaths(regionCount : number, cellId : number) : Promise<number[][]> {
		const route = "/boards/" + regionCount + "/cells/" + cellId + "/paths";
		return await ApiClientCore.sendRequest<{}, number[][]>(
			HttpMethod.Get, route);
	}

}
