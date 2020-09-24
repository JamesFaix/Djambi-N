import { PieceImageInfo } from '../../model/images';
import { ImagesActionTypes } from './actionTypes';

export type PieceImageLoadedAction = {
  type: typeof ImagesActionTypes.PieceImageLoaded,
  info: PieceImageInfo
};

export type ImagesAction = PieceImageLoadedAction;
