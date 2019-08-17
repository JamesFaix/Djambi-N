import * as React from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasPiecesLayer from './canvasPiecesLayer';
import CanvasLabelsLayer from './canvasLabelsLayer';
import CanvasBackgroundLayer from './canvasBackgroundLayer';

export interface CanvasBoardStyle {
    width : number,
    height : number
    scale : number,
    strokeWidth : number,
    strokeColor : string
}

export interface CanvasBoardProps {
    style : CanvasBoardStyle
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    render() {
        const style = this.props.style;

        const stageStyle = {
            width: style.width,
            height: style.height
        };

        const backgroundStyle = {
            strokeWidth: style.strokeWidth,
            strokeColor: style.strokeColor
        };

        const piecesStyle = {
            scale: style.scale
        };

        return (
            <Stage style={stageStyle}>
                <CanvasBackgroundLayer style={backgroundStyle}/>
                <CanvasCellsLayer/>
                <CanvasPiecesLayer style={piecesStyle}/>
                <CanvasLabelsLayer/>
            </Stage>
        );
    }
}