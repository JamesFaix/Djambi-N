import * as React from 'react';
import { Context } from 'konva';
import { Polygon } from '../../../../../boardRendering/model';
import { Shape } from 'react-konva';

export interface CanvasPolygonProps {
    fillColor? : string,
    polygon: Polygon,
    onClick? : () => void
    strokeColor? : string,
    strokeWidth? : number
}

export default class CanvasPolygon extends React.Component<CanvasPolygonProps> {
    render() {
        return (
            <Shape
                sceneFunc={(ctx : Context, shape : any) => {
                    const vs = this.props.polygon.vertices;
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
                fill={this.props.fillColor}
                stroke={this.props.strokeColor}
                strokeWidth={this.props.strokeWidth}
                onClick={() => this.props.onClick()}
            />
        );
    }
}