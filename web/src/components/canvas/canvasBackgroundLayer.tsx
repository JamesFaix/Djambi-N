import * as React from 'react';
import { Layer, Rect } from 'react-konva';
import { Point } from '../../viewModel/board/model';

interface CanvasBackgroundLayerProps {
    size : Point,
    visible : boolean
    color : string,
    clearTooltip : () => void
}

export default class CanvasBackgroundLayer extends React.Component<CanvasBackgroundLayerProps> {
    render() {
        return (
            <Layer>
                <Rect
                    width={this.props.size.x}
                    height={this.props.size.y}
                    fill={this.props.color}
                    onMouseEnter={() => this.props.visible
                        ? this.props.clearTooltip()
                        : null}
                />
            </Layer>
        );
    }
}