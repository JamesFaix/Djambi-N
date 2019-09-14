import * as React from 'react';
import { Layer } from 'react-konva';
import { BoardView } from '../../viewModel/board/model';
import CanvasPolygon, { CanvasPolygonStyle } from './canvasPolygon';

const CanvasBoardOutlineLayer : React.SFC<{
    board : BoardView,
    style : CanvasPolygonStyle
}> = props => (
    <Layer>
        <CanvasPolygon
            polygon={props.board.polygon}
            style={props.style}
        />
    </Layer>
);
export default CanvasBoardOutlineLayer;