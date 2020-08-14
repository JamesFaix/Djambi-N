import { ActiveGameActionTypes } from './actionTypes';
import { GameDto } from '../../api-client';

export type GameLoadedAction = {
  type: typeof ActiveGameActionTypes.GameLoaded,
  game: GameDto
};

export type ActiveGameAction =
  GameLoadedAction;
