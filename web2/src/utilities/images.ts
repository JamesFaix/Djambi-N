import { PieceKind } from '../api-client';

export function getPieceImageKey(kind : PieceKind, colorId : number | null) : string {
  return colorId !== null ? `${kind}${colorId}` : `${kind}Neutral`;
}
