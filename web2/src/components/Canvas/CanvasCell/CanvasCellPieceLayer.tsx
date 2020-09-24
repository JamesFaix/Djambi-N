import React, { FC } from 'react';
import { Image } from 'react-konva';
import { CellView } from '../../../board/model';
import * as Point from '../../../board/point';
import * as Cell from '../../../board/cell';

interface Props {
  size: number,
  image: HTMLImageElement | null,
  cell: CellView
}

const CanvasCellPieceLayer : FC<Props> = ({ size, image, cell }) => {
  if (!cell.piece || !image) {
    return null;
  }

  const cellCenter = Cell.centroid(cell);
  const offset = { x: -(size / 2), y: -(size / 2) };
  const pieceLocation = Point.add(cellCenter, offset);

  return (
    <Image
      image={image}
      x={pieceLocation.x}
      y={pieceLocation.y}
      height={size}
      width={size}
      shadowColor="black"
      shadowOpacity={0.5}
      opacity={1}
      shadowBlur={5}
      shadowOffsetX={5}
      shadowOffsetY={5}
    />
  );
};
export default CanvasCellPieceLayer;
