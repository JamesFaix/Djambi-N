import { BoardDto } from '../../api-client';
import { BoardsActionTypes } from './actionTypes';

export type BoardLoadedAction = {
  type: typeof BoardsActionTypes.BoardLoaded,
  board: BoardDto,
};

export type BoardsAction = BoardLoadedAction;
