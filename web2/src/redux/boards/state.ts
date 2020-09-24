import { BoardDto } from '../../api-client';
import { BoardView } from '../../board/model';

export type BoardsState = {
  boards: Map<number, BoardDto>;
  emptyBoardViews: Map<number, BoardView>;
};

export const defaultBoardsState : BoardsState = {
  boards: new Map<number, BoardDto>(),
  emptyBoardViews: new Map<number, BoardView>(),
};
