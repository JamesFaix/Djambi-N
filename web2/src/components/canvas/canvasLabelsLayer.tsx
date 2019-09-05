import * as React from 'react';
import { Layer } from 'react-konva';
import { CellView, BoardView } from '../../viewModel/board/model';
import Debug from '../../debug';
import CanvasLabel from './canvasLabel';
import { Theme } from '../../themes/model';

export interface CanvasLabelsLayerProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void,
    theme : Theme
}

export default class CanvasLabelsLayer extends React.Component<CanvasLabelsLayerProps> {
    render() {
        if (!Debug.showCellLabels){
            return null;
        }

        const board = this.props.board;

        return (
            <Layer>
                {
                    board.cells.map((c, i) =>
                        <CanvasLabel
                            key={"label" + i}
                            board={board}
                            cell={c}
                            onClick={() => this.props.selectCell(c)}
                            theme={this.props.theme}
                        />
                    )
                }
            </Layer>
        );
    }
}