import { PieceKind } from '../api-client';

export type PieceImageInfo = {
  kind: PieceKind,
  playerColorId: number | null,
  image: HTMLImageElement
};
