import React, { FC, useState } from 'react';
import { Stage } from 'react-konva';
import { useSelector } from 'react-redux';
import CanvasCellsLayer from './CanvasCellsLayer';
import CanvasBoardOutlineLayer from './CanvasBoardOutlineLayer';
import CanvasBackgroundLayer from './CanvasBackgroundLayer';
import CanvasTooltip from './CanvasTooltip';
import { BoardTooltipState, defaultBoardTooltipState } from './model';
import { GameDto } from '../../api-client';
import { BoardView, CellView } from '../../board/model';
import { colors, outlineThickness } from './styles';
import { selectConfig } from '../../hooks/selectors';

interface CanvasBoardStyle {
  width : number,
  height : number
  scale : number,
  strokeWidth : number,
}

interface Props {
  game : GameDto,
  board : BoardView,
  selectCell : (cell : CellView) => void,
  style : CanvasBoardStyle,
  pieceImages : Map<string, HTMLImageElement>,
}

const CanvasBoard : FC<Props> = ({
  board, selectCell, style, pieceImages,
}) => {
  const [tooltipState, setTooltipState] = useState(defaultBoardTooltipState);
  const { showBoardTooltips } = useSelector(selectConfig).user;

  function updateTooltip(state : BoardTooltipState) {
    if (showBoardTooltips) {
      setTooltipState(state);
    }
  }

  function clearTooltip() {
    if (showBoardTooltips) {
      setTooltipState(defaultBoardTooltipState);
    }
  }

  return (
    <Stage
      className={Classes.canvasBoard}
      width={style.width}
      height={style.height}
    >
      <CanvasBackgroundLayer
        size={{ x: style.width, y: style.height }}
        onMouseEnter={clearTooltip}
      />
      <CanvasBoardOutlineLayer
        board={board}
        style={{
          strokeColor: colors.outline,
          strokeWidth: outlineThickness,
        }}
      />
      <CanvasCellsLayer
        board={board}
        selectCell={selectCell}
        pieceImages={pieceImages}
        scale={style.scale}
        setTooltip={updateTooltip}
        showBoardTooltip={showBoardTooltips}
      />
      <CanvasTooltip
        visible={tooltipState.visible && showBoardTooltips}
        text={tooltipState.text}
        position={tooltipState.position}
      />
    </Stage>
  );
};
