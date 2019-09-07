import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCellHighlight from './canvasCellHighlight';
import { CellView, BoardView, CellState } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';
import { Animation } from 'konva';
import { AnimationFrame } from './model';

export interface CanvasHighlightsLayerProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    theme : Theme
}

interface CanvasHighlightsLayerState {
    opacity : number
}

export default class CanvasHighlightsLayer extends React.Component<CanvasHighlightsLayerProps, CanvasHighlightsLayerState> {
    constructor(props : CanvasHighlightsLayerProps) {
        super(props);
        this.state = {
            opacity: 0
        };
    }

    componentDidMount(){
        const period = 0.5; //sec
        const maxOpactiy = 0.5;

        const a = new Animation((frame: AnimationFrame) => {
            const timeSec = frame.time / 1000;
            const opacity = Math.abs(Math.sin(timeSec/period)) * maxOpactiy;
            this.setState({opacity});
        });

        a.start();
    }

    render() {
        const selectableCells = this.props.board.cells
            .filter(c => c.state === CellState.Selectable);

        return (
            <Layer
                opacity={this.state.opacity}
            >
                {selectableCells.map((c, i) =>
                    <CanvasCellHighlight
                        key={i}
                        cell={c}
                        selectCell={(cell) => this.props.selectCell(cell)}
                        theme={this.props.theme}
                    />
                )}
            </Layer>
        );
    }
}