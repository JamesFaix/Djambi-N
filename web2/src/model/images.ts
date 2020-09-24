import { PieceKind } from '../api-client';

export type PieceImageInfo = {
  kind: PieceKind,
  playerColorId: number | null,
  image: HTMLImageElement
};

export function getPieceImageKey(kind: PieceKind, colorId: number | null) : string {
  return colorId !== null ? `${kind}${colorId}` : `${kind}Neutral`;
}
