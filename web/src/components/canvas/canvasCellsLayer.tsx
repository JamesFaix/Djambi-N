import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCell from './canvasCell';
import { BoardView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

export interface CanvasCellsLayerProps {
    gameId : number,
    board : BoardView,
    theme : Theme
}

export default class CanvasCellsLayer extends React.Component<CanvasCellsLayerProps> {
    render() {
        return (
            <Layer>
                {this.props.board.cells.map((c, i) =>
                    <CanvasCell
                        key={i}
                        cell={c}
                        theme={this.props.theme}
                    />
                )}
            </Layer>
        );
    }
}