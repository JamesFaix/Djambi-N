import * as React from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasPiecesLayer from './canvasPiecesLayer';
import CanvasLabelsLayer from './canvasLabelsLayer';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import { BoardView, CellView } from '../../viewModel/board/model';
import { PieceKind } from '../../api/model';
import { Classes } from '../../styles/styles';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';
import CanvasHighlightsLayer from './canvasHighlightsLayer';

export interface CanvasBoardStyle {
    width : number,
    height : number
    scale : number,
    strokeWidth : number,
    theme : Theme
}

export interface CanvasBoardProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasBoardStyle,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    debugSettings : DebugSettings
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {
    render() {
        const style = this.props.style;

        const backgroundStyle = {
            strokeWidth: style.strokeWidth,
            strokeColor: style.theme.colors.cells.boardBorder
        };

        const piecesStyle = {
            scale: style.scale,
            theme: style.theme
        };

        return (
            <Stage
                className={Classes.canvasBoard}
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
                    theme={style.theme}
                />
                <CanvasPiecesLayer
                    board={this.props.board}
                    style={piecesStyle}
                    images={this.props.pieceImages}
                />
                <CanvasLabelsLayer
                    gameId={this.props.gameId}
                    board={this.props.board}
                    theme={this.props.style.theme}
                    debugSettings={this.props.debugSettings}
                />
                <CanvasHighlightsLayer
                    gameId={this.props.gameId}
                    board={this.props.board}
                    selectCell={this.props.selectCell}
                    theme={style.theme}
                />
            </Stage>
        );
    }
}