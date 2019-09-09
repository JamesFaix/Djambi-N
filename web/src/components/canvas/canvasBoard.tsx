import * as React from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasBoardOutlineLayer from './canvasBoardOutlineLayer';
import { BoardView, CellView, Point } from '../../viewModel/board/model';
import { PieceKind, Game } from '../../api/model';
import { Classes } from '../../styles/styles';
import { Theme } from '../../themes/model';
import { AnimationFrame } from './model';
import { Animation } from 'konva';
import { DebugSettings } from '../../debug';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import CanvasTooltip from './canvasTooltip';

export interface CanvasBoardStyle {
    width : number,
    height : number
    scale : number,
    strokeWidth : number,
    theme : Theme
}

export interface CanvasBoardProps {
    game : Game,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasBoardStyle,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    debugSettings : DebugSettings
}

export interface BoardTooltipData {
    text : string,
    position : Point,
    visible : boolean
}

const defaultBoardTooltipData = {
    text: "",
    position: { x: 0, y: 0 },
    visible: false
}

interface CanvasBoardState {
    highlightOpacity : number,
    tooltipData : BoardTooltipData
}

export default class CanvasBoard extends React.Component<CanvasBoardProps, CanvasBoardState> {
    constructor(props : CanvasBoardProps) {
        super(props);
        this.state = {
            highlightOpacity: 0,
            tooltipData: defaultBoardTooltipData
        };
    }

    componentDidMount(){
        const period = 0.5; //sec
        const maxOpactiy = 0.5;

        const a = new Animation((frame: AnimationFrame) => {
            const timeSec = frame.time / 1000;
            const opacity = Math.abs(Math.sin(timeSec/period)) * maxOpactiy;
            this.setState({highlightOpacity: opacity});
        });

        a.start();
    }

    render() {
        const style = this.props.style;

        const outlineStyle = {
            strokeWidth: style.strokeWidth,
            strokeColor: style.theme.colors.cells.boardBorder
        };

        return (
            <Stage
                className={Classes.canvasBoard}
                width={style.width}
                height={style.height}
            >
                <CanvasBackgroundLayer
                    size={{ x: style.width, y: style.height }}
                    color={style. theme.colors.background}
                    clearTooltip={() => this.setState({ tooltipData: defaultBoardTooltipData })}
                    visible={this.props.debugSettings.showCellLabels}
                />
                <CanvasBoardOutlineLayer
                    board={this.props.board}
                    style={outlineStyle}
                />
                <CanvasCellsLayer
                    gameId={this.props.game.id}
                    board={this.props.board}
                    theme={style.theme}
                    selectCell={this.props.selectCell}
                    highlightOpacity={this.state.highlightOpacity}
                    pieceImages={this.props.pieceImages}
                    scale={style.scale}
                    setTooltip={data => this.setState({ tooltipData: data })}
                    game={this.props.game}
                />
                <CanvasTooltip
                    visible={this.state.tooltipData.visible && this.props.debugSettings.showCellLabels}
                    text={this.state.tooltipData.text}
                    position={this.state.tooltipData.position}
                    theme={style.theme}
                />
            </Stage>
        );
    }
}