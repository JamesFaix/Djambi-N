import { PieceKind } from '../api-client';
import { PieceImageInfo } from '../model/images';
import { store } from '../redux';
import { pieceImageLoadedAction } from '../redux/images/actionFactory';
import { replaceColor } from '../utilities/images';

function createPieceImage(kind : PieceKind, colorId : number | null) : HTMLImageElement {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  let image = new (window as any).Image() as HTMLImageElement;
  image.src = ThemeService.getPieceImagePath(theme, kind);
  image.onload = () => {
    const dummyColor = theme.colors.players.placeholder;
    const playerColor = ThemeService.getPlayerColor(theme, colorId);
    image = replaceColor(image, dummyColor, playerColor);

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

function createPieceImageForEachPlayerColor(kind: PieceKind) : HTMLImageElement {
  const minPlayerColorId = 0;
  const maxPlayerColorId = 7;
  for (let colorId = minPlayerColorId; colorId <= maxPlayerColorId; colorId += 1) {
    createPieceImage(kind, colorId);
  }
  Controller.Display.createPieceImage(theme, k, null); // Neutral sprite for abandoned pieces
}

function loadThemeImages(theme : Theme) : void {
  const kinds = [
    PieceKind.Hunter,
    PieceKind.Conduit,
    PieceKind.Diplomat,
    PieceKind.Reaper,
    PieceKind.Scientist,
    PieceKind.Thug,
  ];
  const maxPlayerColorId = 7;

  kinds.forEach((k) => {
    for (let colorId = 0; colorId <= maxPlayerColorId; colorId++) {
      Controller.Display.createPieceImage(theme, k, colorId);
    }
    Controller.Display.createPieceImage(theme, k, null); // Neutral sprite for abandoned pieces
  });

  Controller.Display.createPieceImage(theme, PieceKind.Corpse, null); // Corpses are only ever neutral
}

export function preloadAllPieceImages() : Promise<void> {

}
