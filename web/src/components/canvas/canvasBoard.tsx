import * as React from 'react';
import { Stage } from 'react-konva';
import CanvasCellsLayer from './canvasCellsLayer';
import CanvasBoardOutlineLayer from './canvasBoardOutlineLayer';
import { BoardView, CellView } from '../../viewModel/board/model';
import { PieceKind, Game } from '../../api/model';
import { Classes } from '../../styles/styles';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';
import CanvasBackgroundLayer from './canvasBackgroundLayer';
import CanvasTooltip from './canvasTooltip';
import { BoardTooltipState, defaultBoardTooltipState } from './model';

interface CanvasBoardStyle {
    width : number,
    height : number
    scale : number,
    strokeWidth : number,
    theme : Theme
}

interface Props {
    game : Game,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    style : CanvasBoardStyle,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    debugSettings : DebugSettings
}

interface State {
    tooltipState : BoardTooltipState
}

export default class CanvasBoard extends React.Component<Props, State> {
    constructor(props : Props) {
        super(props);
        this.state = {
            tooltipState: defaultBoardTooltipState
        };
    }

    render() {
        const style = this.props.style;
        const ttState = this.state.tooltipState;

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
                    onMouseEnter={() => this.clearTooltip()}
                />
                <CanvasBoardOutlineLayer
                    board={this.props.board}
                    style={outlineStyle}
                />
                <CanvasCellsLayer
                    board={this.props.board}
                    theme={style.theme}
                    selectCell={this.props.selectCell}
                    pieceImages={this.props.pieceImages}
                    scale={style.scale}
                    setTooltip={state => this.updateTooltip(state)}
                    game={this.props.game}
                    showBoardTooltip={this.props.debugSettings.showBoardTooltips}
                />
                <CanvasTooltip
                    visible={ttState.visible && this.props.debugSettings.showBoardTooltips}
                    text={ttState.text}
                    position={ttState.position}
                    theme={style.theme}
                />
            </Stage>
        );
    }

    private updateTooltip(state : BoardTooltipState) {
        if (this.props.debugSettings.showBoardTooltips) {
            this.setState({ tooltipState: state });
        }
    }

    private clearTooltip() {
        if (this.props.debugSettings.showBoardTooltips) {
            this.setState({ tooltipState: defaultBoardTooltipState });
        }
    }
}