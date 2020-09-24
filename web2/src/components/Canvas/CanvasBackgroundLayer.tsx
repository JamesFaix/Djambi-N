import React, { FC } from 'react';
import { Layer, Rect } from 'react-konva';
import { Point } from '../../board/model';
import { colors } from './styles';

interface Props {
  size : Point,
  onMouseEnter: () => void
}

const CanvasBackgroundLayer : FC<Props> = ({ size, onMouseEnter }) => (
  <Layer>
    <Rect
      width={size.x}
      height={size.y}
      fill={colors.background}
      onMouseEnter={onMouseEnter}
    />
  </Layer>
);
export default CanvasBackgroundLayer;
