import { ActiveGameState, defaultActiveGameState } from './state';
import { ActiveGameActionTypes } from './actionTypes';
import { ActiveGameAction } from './actionts';

export function activeGameReducer(
  state: ActiveGameState = defaultActiveGameState,
  action: ActiveGameAction,
): ActiveGameState {
  switch (action.type) {
    case ActiveGameActionTypes.GameLoaded:
      return {
        ...state,
        game: action.game,
      };

    case ActiveGameActionTypes.GameUpdated:
      return {
        ...state,
        game: action.response.game,
      };

    default:
      return state;
  }
}
