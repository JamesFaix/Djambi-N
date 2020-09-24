import { createEmptyBoardView } from '../../board/boardViewFactory';
import { add } from '../../utilities/collections';
import { BoardsAction } from './actions';
import { BoardsActionTypes } from './actionTypes';
import { BoardsState, defaultBoardsState } from './state';

export function boardsReducer(
  state: BoardsState = defaultBoardsState,
  action : BoardsAction,
): BoardsState {
  switch (action.type) {
    case BoardsActionTypes.BoardLoaded: {
      const { boards, emptyBoardViews } = state;
      const { board } = action;
      const { regionCount } = board;
      const view = createEmptyBoardView(board);

      return {
        ...state,
        boards: add(boards, regionCount, board),
        emptyBoardViews: add(emptyBoardViews, regionCount, view),
      };
    }
    default:
      return state;
  }
}
