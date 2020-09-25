import { PieceKind } from '../api-client';
import { PieceImageInfo } from '../model/images';
import { store } from '../redux';
import { pieceImageLoadedAction } from '../redux/images/actionFactory';
import { pieceColors } from '../styles/styles';
import { replaceColor } from '../utilities/images';

const minPlayerColorId = 0;
const maxPlayerColorId = 7;

const piecesDir = '../assets/pieces';

function getPieceImagePath(kind: PieceKind) : string {
  switch (kind) {
    case PieceKind.Conduit: return `${piecesDir}/conduit.png`;
    case PieceKind.Corpse: return `${piecesDir}/corpse.png`;
    case PieceKind.Diplomat: return `${piecesDir}/diplomat.png`;
    case PieceKind.Hunter: return `${piecesDir}/hunter.png`;
    case PieceKind.Reaper: return `${piecesDir}/reaper.png`;
    case PieceKind.Scientist: return `${piecesDir}/scientist.png`;
    case PieceKind.Thug: return `${piecesDir}/thug.png`;
    default: throw Error(`Invalid piece kind: ${kind}`);
  }
}

function createPieceImage(kind : PieceKind, colorId : number | null) : HTMLImageElement {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  let image = new (window as any).Image() as HTMLImageElement;
  image.src = getPieceImagePath(kind);
  image.onload = () => {
    image = replaceColor(image, pieceColors.placeholder, pieceColors.getPlayer(colorId));

    const info : PieceImageInfo = {
      kind,
      playerColorId: colorId,
      image,
    };

    const action = pieceImageLoadedAction(info);
    store.dispatch(action);
  };
  return image;
}

function createPieceImageForEachPlayerColor(kind: PieceKind) : void {
  for (let colorId = minPlayerColorId; colorId <= maxPlayerColorId; colorId += 1) {
    createPieceImage(kind, colorId);
  }
  createPieceImage(kind, null); // Neutral sprite for abandoned pieces
}

export async function preloadAllPieceImages() : Promise<void> {
  const kinds = [
    PieceKind.Hunter,
    PieceKind.Conduit,
    PieceKind.Diplomat,
    PieceKind.Reaper,
    PieceKind.Scientist,
    PieceKind.Thug,
  ];

  kinds.forEach((k) => createPieceImageForEachPlayerColor(k));

  createPieceImage(PieceKind.Corpse, null); // Corpses are only ever neutral
}
