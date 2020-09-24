import React, { FC, useState } from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './CanvasCellsLayer';
import CanvasBoardOutlineLayer from './CanvasBoardOutlineLayer';
import CanvasBackgroundLayer from './CanvasBackgroundLayer';
import CanvasTooltip from './CanvasTooltip';
import { BoardTooltipState, defaultBoardTooltipState } from './model';
import { GameDto } from '../../api-client';
import { BoardView, CellView } from '../../board/model';

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
  debugSettings : DebugSettings
}

const CanvasBoard : FC<Props> = ({
  board, selectCell, style, pieceImages, debugSettings,
}) => {
  const [tooltipState, setTooltipState] = useState(defaultBoardTooltipState);

  const outlineStyle = {
    strokeWidth: style.strokeWidth,
    strokeColor: style.theme.colors.cells.boardBorder,
  };

  function updateTooltip(state : BoardTooltipState) {
    if (debugSettings.showBoardTooltips) {
      setTooltipState(state);
    }
  }

  function clearTooltip() {
    if (debugSettings.showBoardTooltips) {
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
        style={outlineStyle}
      />
      <CanvasCellsLayer
        board={board}
        selectCell={selectCell}
        pieceImages={pieceImages}
        scale={style.scale}
        setTooltip={updateTooltip}
        showBoardTooltip={debugSettings.showBoardTooltips}
      />
      <CanvasTooltip
        visible={tooltipState.visible && debugSettings.showBoardTooltips}
        text={tooltipState.text}
        position={tooltipState.position}
      />
    </Stage>
  );
};
