import * as React from 'react';
import { Context } from 'konva';
import { Polygon } from '../../viewModel/board/model';
import { Shape } from 'react-konva';

export interface CanvasPolygonStyle {
    fillColor ?: string,
    strokeColor ?: string,
    strokeWidth ?: number
}

export interface CanvasPolygonProps {
    polygon : Polygon,
    onClick ?: () => void,
    style: CanvasPolygonStyle
}

export default class CanvasPolygon extends React.Component<CanvasPolygonProps> {
    render() {
        const p = this.props;
        const style = p.style;
        return (
            <Shape
                sceneFunc={(ctx : Context, shape : any) => {
                    const vs = p.polygon.vertices;
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
                fill={style.fillColor}
                stroke={style.strokeColor}
                strokeWidth={style.strokeWidth}
                onClick={p.onClick ? () => p.onClick() : null}
            />
        );
    }
}