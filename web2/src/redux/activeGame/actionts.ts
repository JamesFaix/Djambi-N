import { ActiveGameActionTypes } from './actionTypes';
import { GameDto, StateAndEventResponseDto } from '../../api-client';

export type GameLoadedAction = {
  type: typeof ActiveGameActionTypes.GameLoaded,
  game: GameDto
};

export type GameUpdatedAction = {
  type: typeof ActiveGameActionTypes.GameUpdated,
  response: StateAndEventResponseDto
};

export type ActiveGameAction =
  GameLoadedAction |
  GameUpdatedAction;
