import * as React from 'react';
import CanvasBoard, { CanvasBoardStyle } from '../canvas/canvasBoard';

export interface BoardSectionProps {

}

export default class BoardSection extends React.Component<BoardSectionProps> {
    render(){
        const boardStyle : CanvasBoardStyle = {
            width: 1000, //get from CanvasTransformService
            height : 1000, //get from CanvasTransformService
            strokeWidth : 5, //5 maybe put in settings somewhere
            strokeColor : "black", //pull from Colors module
            scale : 1 //get from CanvasTransformService
        };

        return (
            <div>
                <CanvasBoard
                    style={boardStyle}
                />
            </div>
        );
    }
}