import * as React from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasPiecesLayer from './canvasPiecesLayer';
import CanvasLabelsLayer from './canvasLabelsLayer';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import { BoardView, CellView } from '../../viewModel/board/model';

export interface CanvasBoardStyle {
    width : number,
    height : number
    scale : number,
    strokeWidth : number,
    strokeColor : string
}

export interface CanvasBoardProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasBoardStyle
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    render() {
        const style = this.props.style;

        const backgroundStyle = {
            strokeWidth: style.strokeWidth,
            strokeColor: style.strokeColor
        };

        const piecesStyle = {
            scale: style.scale
        };

        return (
            <Stage
                width={style.width}
                height={style.height}
            >
                <CanvasBackgroundLayer
                    board={this.props.board}
                    style={backgroundStyle}
                />
                <CanvasCellsLayer
                    gameId={this.props.gameId}
                    board={this.props.board}
                    selectCell={this.props.selectCell}
                />
                <CanvasPiecesLayer
                    board={this.props.board}
                    selectCell={this.props.selectCell}
                    style={piecesStyle}
                />
                <CanvasLabelsLayer
                    gameId={this.props.gameId}
                    board={this.props.board}
                    selectCell={this.props.selectCell}
                />
            </Stage>
        );
    }
}