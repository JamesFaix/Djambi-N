import { ActiveGameActionTypes } from './actionTypes';
import { GameDto, StateAndEventResponseDto } from '../../api-client';
import { GameLoadedAction, GameUpdatedAction, AttemptingGameLoadAction } from './actions';

export function attemptingGameLoad(): AttemptingGameLoadAction {
  return {
    type: ActiveGameActionTypes.AttemptingGameLoad,
  };
}

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
