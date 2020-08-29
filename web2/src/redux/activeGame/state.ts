import { GameDto } from '../../api-client';

export type ActiveGameState = {
  game: GameDto | null,
  isLoadPending: boolean,
};

export const defaultActiveGameState: ActiveGameState = {
  game: null,
  isLoadPending: false,
};
