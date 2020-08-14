import { ActiveGameActionTypes } from './actionTypes';
import { GameDto, StateAndEventResponseDto } from '../../api-client';
import { GameLoadedAction, GameUpdatedAction } from './actionts';

export function gameLoaded(game: GameDto): GameLoadedAction {
  return {
    type: ActiveGameActionTypes.GameLoaded,
    game,
  };
}

export function gameUpdated(response: StateAndEventResponseDto): GameUpdatedAction {
  return {
    type: ActiveGameActionTypes.GameUpdated,
    response,
  };
}
