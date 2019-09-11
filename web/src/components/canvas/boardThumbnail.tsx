import * as React from 'react';
import { Stage, Layer } from 'react-konva';
import CanvasBoardOutlineLayer from './canvasBoardOutlineLayer';
import { Classes } from '../../styles/styles';
import { Theme } from '../../themes/model';
import CanvasCellBackgroundLayer from './canvasCellBackgroundLayer';
import { Board } from '../../api/model';
import BoardViewFactory from '../../viewModel/board/boardViewFactory';
import { Point } from '../../viewModel/board/model';
import Geometry from '../../viewModel/board/geometry';
import CanvasTransformService from '../../viewModel/board/canvasTransformService';

const BoardThumbnail : React.SFC<{
    board : Board,
    size : Point,
    strokeWidth : number,
    theme : Theme
}> = props => {
    let bv = BoardViewFactory.createEmptyBoardView(props.board);
    let t = CanvasTransformService.getBoardViewTransform({
        containerSize: props.size,
        canvasMargin: props.strokeWidth * 2,
        contentPadding: 0,
        regionCount: props.board.regionCount,
        zoomLevel: 0
    });
    bv = Geometry.Board.transform(bv, t);
    return (
        <Stage
            className={Classes.canvasBoard}
            width={props.size.x}
            height={props.size.y}
        >
            <CanvasBoardOutlineLayer
                board={bv}
                style={{
                    strokeWidth: props.strokeWidth,
                    strokeColor: props.theme.colors.cells.boardBorder
                }}
            />
            <Layer>
                {bv.cells.map((c, i) =>
                    <CanvasCellBackgroundLayer
                        key={i}
                        cell={c}
                        theme={props.theme}
                    />
                )}
            </Layer>
        </Stage>
    );
}
export default BoardThumbnail;