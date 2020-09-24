import { Point } from '../../board/model';

export interface BoardTooltipState {
  text : string,
  position : Point,
  visible : boolean
}

export const defaultBoardTooltipState = {
  text: '',
  position: { x: 0, y: 0 },
  visible: false,
};
