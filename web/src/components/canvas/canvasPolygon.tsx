import * as React from 'react';
import { Context } from 'konva';
import { Polygon } from '../../viewModel/board/model';
import { Shape } from 'react-konva';

export interface CanvasPolygonStyle {
    fillColor ?: string,
    strokeColor ?: string,
    strokeWidth ?: number,
    opacity ?: number
}

const CanvasPolygon : React.SFC<{
    polygon : Polygon,
    style: CanvasPolygonStyle
}> = props => (
    <Shape
        sceneFunc={(ctx : Context, shape : any) => {
            const vs = props.polygon.vertices;
            let v = vs[0];

            ctx.beginPath();
            ctx.moveTo(v.x, v.y);

            for (var i = 1; i < vs.length; i++) {
                v = vs[i];
                ctx.lineTo(v.x, v.y);
            }

            ctx.closePath();
            ctx.fillStrokeShape(shape);
        }}
        fill={props.style.fillColor}
        stroke={props.style.strokeColor}
        strokeWidth={props.style.strokeWidth}
        opacity={props.style.opacity !== undefined ? props.style.opacity : 1}
    />
);
export default CanvasPolygon;