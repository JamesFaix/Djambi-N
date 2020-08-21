import * as Api from '../utilities/api';
import { gameLoaded, gameUpdated } from '../redux/activeGame/actionFactory';
import { store } from '../redux';
import { GameParametersDto, CreatePlayerRequestDto } from '../api-client';
import * as Routes from '../utilities/routes';
import { navigateTo } from '../utilities/navigation';

export async function loadGame(gameId: number): Promise<void> {
  const game = await Api.games().apiGamesGameIdGet({ gameId });
  const action = gameLoaded(game);
  store.dispatch(action);
}

export async function createGame(parameters: GameParametersDto): Promise<void> {
  const game = await Api.games().apiGamesPost({ gameParametersDto: parameters });
  const action = gameLoaded(game);
  store.dispatch(action);
  navigateTo(Routes.gameLobby(game.id));
}

export async function addPlayer(gameId: number, request: CreatePlayerRequestDto): Promise<void> {
  const postPlayerRequest = { gameId, createPlayerRequestDto: request };
  const response = await Api.players().apiGamesGameIdPlayersPost(postPlayerRequest);
  const action = gameUpdated(response);
  store.dispatch(action);
}

export async function removePlayer(gameId: number, playerId: number): Promise<void> {
  const deletePlayerRequest = { gameId, playerId };
  const response = await Api.players().apiGamesGameIdPlayersPlayerIdDelete(deletePlayerRequest);
  const action = gameUpdated(response);
  store.dispatch(action);
}
