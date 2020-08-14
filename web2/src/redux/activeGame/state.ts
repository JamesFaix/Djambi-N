import { GameDto } from '../../api-client';

export type ActiveGameState = {
  game: GameDto | null
};

export const defaultActiveGameState: ActiveGameState = {
  game: null,
};
