import * as React from 'react';
import { Stage, Label, Tag, Text, Layer, Rect } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import { BoardView, CellView, Point } from '../../viewModel/board/model';
import { PieceKind, Game } from '../../api/model';
import { Classes } from '../../styles/styles';
import { Theme } from '../../themes/model';
import { AnimationFrame } from './model';
import { Animation } from 'konva';
import { stripLeadingSlash } from 'history/PathUtils';

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
}

export interface BoardTooltipData {
    text : string,
    position : Point,
    visible : boolean
}

export const defaultBoardTooltipData = {
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

        const backgroundStyle = {
            strokeWidth: style.strokeWidth,
            strokeColor: style.theme.colors.cells.boardBorder
        };

        return (
            <Stage
                className={Classes.canvasBoard}
                width={style.width}
                height={style.height}
            >
                <Layer>
                    <Rect
                        width={style.width}
                        height={style.height}
                        fill={style.theme.colors.background}
                        onMouseEnter={() => this.setState({ tooltipData: defaultBoardTooltipData })}
                    />
                </Layer>

                <CanvasBackgroundLayer
                    board={this.props.board}
                    style={backgroundStyle}
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
                {this.renderTooltip()}
            </Stage>
        );
    }

    private renderTooltip() {
        const data = this.state.tooltipData;
        if (!data.visible) {
            return null;
        }

        const colors = this.props.style.theme.colors;

        return (
            <Layer>
                <Label
                    x={data.position.x}
                    y={data.position.y}
                    opacity={1}
                >
                    <Tag
                        stroke={colors.border}
                        strokeWidth={1}
                        fill={colors.background}
                        shadowColor={"black"}
                        shadowBlur={5}
                        shadowOpacity={1}
                    />
                    <Text
                        text={data.text}
                        fill={colors.text}
                        padding={5}
                    />
                </Label>
            </Layer>
        );
    }
}