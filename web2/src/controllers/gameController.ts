import * as Api from '../utilities/api';
import { gameLoaded } from '../redux/activeGame/actionFactory';
import { store } from '../redux';

export async function loadGame(gameId: number): Promise<void> {
  const game = await Api.games().apiGamesGameIdGet({ gameId });
  const action = gameLoaded(game);
  store.dispatch(action);
}
