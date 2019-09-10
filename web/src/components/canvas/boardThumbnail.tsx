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

interface Props {
    board : Board,
    size : Point,
    strokeWidth : number,
    theme : Theme
}

export default class BoardThumbnail extends React.Component<Props> {
    render() {
        const p = this.props;
        let bv = BoardViewFactory.createEmptyBoardView(p.board);
        let t = CanvasTransformService.getBoardViewTransform({
            containerSize: p.size,
            canvasMargin: p.strokeWidth * 2,
            contentPadding: 0,
            regionCount: p.board.regionCount,
            zoomLevel: 0
        });
        bv = Geometry.Board.transform(bv, t);
        return (
            <Stage
                className={Classes.canvasBoard}
                width={p.size.x}
                height={p.size.y}
            >
                <CanvasBoardOutlineLayer
                    board={bv}
                    style={{
                        strokeWidth: p.strokeWidth,
                        strokeColor: p.theme.colors.cells.boardBorder
                    }}
                />
                <Layer>
                    {bv.cells.map((c, i) =>
                        <CanvasCellBackgroundLayer
                            key={i}
                            cell={c}
                            theme={p.theme}
                        />
                    )}
                </Layer>
            </Stage>
        );
    }
}