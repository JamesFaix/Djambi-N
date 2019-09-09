import * as React from 'react';
import { Layer } from 'react-konva';
import { BoardView } from '../../viewModel/board/model';
import CanvasPolygon, { CanvasPolygonStyle } from './canvasPolygon';

export interface CanvasBoardOutlineLayerProps {
    board : BoardView,
    style : CanvasPolygonStyle
}

export default class CanvasBoardOutlineLayer extends React.Component<CanvasBoardOutlineLayerProps> {
    render() {
        return (
            <Layer>
                <CanvasPolygon
                    polygon={this.props.board.polygon}
                    style={this.props.style}
                />
            </Layer>
        );
    }
}