import React, { FC } from 'react';
import { Layer } from 'react-konva';
import { BoardView } from '../../board/model';
import CanvasPolygon, { CanvasPolygonStyle } from './CanvasPolygon';

interface Props {
  board: BoardView,
  style: CanvasPolygonStyle
}

const CanvasBoardOutlineLayer : FC<Props> = ({ board, style }) => {
  return (
    <Layer>
      <CanvasPolygon
        polygon={board.polygon}
        style={style}
      />
    </Layer>
  );
};
export default CanvasBoardOutlineLayer;
