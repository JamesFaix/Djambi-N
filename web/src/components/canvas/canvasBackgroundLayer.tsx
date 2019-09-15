import * as React from 'react';
import { Layer, Rect } from 'react-konva';
import { Point } from '../../viewModel/board/model';

const CanvasBackgroundLayer : React.SFC<{
    size : Point,
    color : string,
    onMouseEnter : () => void
}> = props => (
    <Layer>
        <Rect
            width={props.size.x}
            height={props.size.y}
            fill={props.color}
            onMouseEnter={() => props.onMouseEnter()}
        />
    </Layer>
);
export default CanvasBackgroundLayer