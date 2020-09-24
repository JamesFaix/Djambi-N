import { PieceImageInfo } from '../../model/images';
import { PieceImageLoadedAction } from './actions';
import { ImagesActionTypes } from './actionTypes';

export function pieceImageLoadedAction(info : PieceImageInfo) : PieceImageLoadedAction {
  return {
    type: ImagesActionTypes.PieceImageLoaded,
    info,
  };
}
