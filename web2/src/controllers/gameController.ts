import * as Api from '../utilities/api';
import { gameLoaded } from '../redux/activeGame/actionFactory';
import { store } from '../redux';
import { GameParametersDto } from '../api-client';
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
