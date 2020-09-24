import React, { FC } from 'react';
import { CellView } from '../../../board/model';
import CanvasPolygon from '../CanvasPolygon';
import { getCellColor, getCellBorderColor } from '../styles';

interface Props {
  cell: CellView,
}

const CanvasCellBackgroundLayer : FC<Props> = ({ cell }) => {
  const color = getCellColor(cell);
  let borderColor = getCellBorderColor(cell);
  if (!borderColor) {
    borderColor = color;
  }

  return (
    <CanvasPolygon
      polygon={cell.polygon}
      style={{
        fillColor: color,
        strokeColor: borderColor,
        // Stroke is necessary to fill gaps between polygons belonging to the same cell
        strokeWidth: 1,
      }}
    />
  );
};
export default CanvasCellBackgroundLayer;
