import React, { FC, useEffect, useState } from 'react';
import { Layer } from 'react-konva';
import { Animation } from 'konva';
import { BoardView, CellView, PieceView } from '../../board/model';
import { AnimationFrame, BoardTooltipState } from './model';
import CanvasCell from './CanvasCell/CanvasCell';
import { getBoardPieceScale } from '../../board/canvasTransformService';
import { getPieceImageKey } from '../../utilities/images';

interface Props {
  board : BoardView,
  selectCell : (cell : CellView) => void,
  pieceImages : Map<string, HTMLImageElement>,
  scale : number,
  setTooltip : (state : BoardTooltipState) => void,
  showBoardTooltip : boolean
}

const CanvasCellsLayer : FC<Props> = ({
  board, selectCell, pieceImages, scale, setTooltip, showBoardTooltip,
}) => {
  function getPieceImage(piece : PieceView | null) : HTMLImageElement | null {
    if (!piece) { return null; }
    const key = getPieceImageKey(piece.kind, piece.colorId);
    return pieceImages.get(key) || null;
  }

  const [highlightOpacity, setHighlightOpacity] = useState(0);

  useEffect(() => {
    const period = 0.5; // sec
    const maxOpactiy = 0.5;

    const a = new Animation((frame: AnimationFrame) => {
      const timeSec = frame.time / 1000;
      const opacity = Math.abs(Math.sin(timeSec / period)) * maxOpactiy;
      setHighlightOpacity(opacity);
    });

    a.start();
  });

  const pieceSize = scale * getBoardPieceScale(board);
  return (
    <Layer>
      {board.cells.map((c, i) => (
        <CanvasCell
          key={i.toString()}
          cell={c}
          highlightOpacity={c.isSelectable ? highlightOpacity : 0}
          selectCell={selectCell}
          pieceSize={pieceSize}
          pieceImage={getPieceImage(c.piece)}
          setTooltip={setTooltip}
          showBoardTooltip={showBoardTooltip}
        />
      ))}
    </Layer>
  );
};
export default CanvasCellsLayer;
