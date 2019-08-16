import * as React from 'react';
import CanvasCell from './canvasCell';
import CanvasLabel from './canvasLabel';
import CanvasPiece from './canvasPiece';
import CanvasPolygon from './canvasPolygon';
import Debug from '../../debug';
import Geometry from '../../viewModel/board/geometry';
import { BoardView, CellType, CellView } from '../../viewModel/board/model';
import { Layer, Stage } from 'react-konva';
import { Point } from '../../viewModel/board/model';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasPiecesLayer from './canvasPiecesLayer';
import CanvasLabelsLayer from './canvasLabelsLayer';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import Colors from '../../utilities/colors';

export interface CanvasBoardProps {
    board : BoardView,
    selectCell : (cell : CellView) => void,
    scale : number,
    boardStrokeWidth : number,
    size : Point
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    render() {
        return (
            <Stage style={{width: this.props.size.x, height: this.props.size.y}}>
                <CanvasBackgroundLayer
                    board={this.props.board}
                    style={{
                        strokeWidth: this.props.boardStrokeWidth,
                        strokeColor: Colors.getBoardBorderColor()
                    }}
                />
                <CanvasCellsLayer
                    board={this.props.board}
                    selectCell={cell => this.props.selectCell(cell)}
                />
                <CanvasPiecesLayer
                    board={this.props.board}
                    selectCell={cell => this.props.selectCell(cell)}
                    scale={this.props.scale}
                />
                <CanvasLabelsLayer
                    board={this.props.board}
                    selectCell={cell => this.props.selectCell(cell)}
                />
            </Stage>
        );
    }
}