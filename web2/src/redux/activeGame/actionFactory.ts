import { ActiveGameActionTypes } from './actionTypes';
import { GameDto } from '../../api-client';
import { GameLoadedAction } from './actionts';

export function gameLoaded(game: GameDto): GameLoadedAction {
  return {
    type: ActiveGameActionTypes.GameLoaded,
    game,
  };
}
