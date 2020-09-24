import { LocationDto, PieceKind } from '../api-client';
import { CellView, PieceView } from '../board/model';
import { store } from '../redux';

export const centerCellName = 'The Apex';

export function locationToString(location: LocationDto) : string {
  return `(${location.region}, ${location.x}, ${location.y})`;
}

function showIds() : boolean {
  return store.getState().config.user.showCellAndPieceIds;
}

export function getCellViewLabel(cell: CellView): string {
  const base = cell.locations.find((l) => l.x === 0 && l.y === 0)
    ? centerCellName
    : locationToString(cell.locations[0]);

  return showIds()
    ? `${base} (#${cell.id})`
    : base;
}

export function getPieceKindName(kind: PieceKind) : string {
  switch (kind) {
    case PieceKind.Conduit: return 'Conduit';
    case PieceKind.Corpse: return 'Corpse';
    case PieceKind.Diplomat: return 'Diplomat';
    case PieceKind.Hunter: return 'Hunter';
    case PieceKind.Reaper: return 'Reaper';
    case PieceKind.Scientist: return 'Scientist';
    case PieceKind.Thug: return 'Thug';
    default: throw Error(`Invalid piece kind ${kind}.`);
  }
}

export function getPieceViewLabel(piece: PieceView) : string {
  const kindName = getPieceKindName(piece.kind);

  if (piece.kind === PieceKind.Corpse) {
    return kindName;
  }

  const base = piece.playerName
    ? `${piece.playerName}'s ${kindName}`
    : `Neutral ${kindName}`;

  return showIds()
    ? `${base} (#${piece.id})`
    : base;
}
