import { BoardDto } from '../../api-client';
import { BoardLoadedAction } from './actions';
import { BoardsActionTypes } from './actionTypes';

export function boardLoadedAction(board: BoardDto) : BoardLoadedAction {
  return {
    type: BoardsActionTypes.BoardLoaded,
    board,
  };
}
