import { ActiveGameState, defaultActiveGameState } from './state';
import { ActiveGameActionTypes } from './actionTypes';
import { ActiveGameAction } from './actions';

export function activeGameReducer(
  state: ActiveGameState = defaultActiveGameState,
  action: ActiveGameAction,
): ActiveGameState {
  switch (action.type) {
    case ActiveGameActionTypes.AttemptingGameLoad:
      return {
        ...state,
        isLoadPending: true,
      };

    case ActiveGameActionTypes.GameLoaded:
      return {
        ...state,
        game: action.game,
        isLoadPending: false,
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
