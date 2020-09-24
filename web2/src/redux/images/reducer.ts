import { add } from '../../utilities/collections';
import { getPieceImageKey } from '../../utilities/images';
import { ImagesAction } from './actions';
import { ImagesActionTypes } from './actionTypes';
import { defaultImagesState, ImagesState } from './state';

export function imagesReducer(
  state: ImagesState = defaultImagesState,
  action : ImagesAction,
) : ImagesState {
  switch (action.type) {
    case ImagesActionTypes.PieceImageLoaded:
    {
      const { image, kind, playerColorId } = action.info;
      const key = getPieceImageKey(kind, playerColorId);
      return {
        ...state,
        pieces: add(state.pieces, key, image),
      };
    }

    default:
      return state;
  }
}
