import React, { FC } from 'react';
import { CellView } from '../../../board/model';
import CanvasPolygon from '../CanvasPolygon';
import { colors } from '../styles';

interface Props {
  cell: CellView,
  opacity: number
}

const CanvasCellHighlightLayer : FC<Props> = ({ cell, opacity }) => {
  if (!cell.isSelectable) {
    return null;
  }

  return (
    <CanvasPolygon
      polygon={cell.polygon}
      style={{
        fillColor: colors.selectableCell,
        opacity,
      }}
    />
  );
};
export default CanvasCellHighlightLayer;
