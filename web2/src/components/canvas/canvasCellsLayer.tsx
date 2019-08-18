import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCell from './canvasCell';
import { CellView, BoardView } from '../../viewModel/board/model';

export interface CanvasCellsLayerProps {
    gameId : number,
    board : BoardView,
    selectCell : (cell : CellView) => void
}

export default class CanvasCellsLayer extends React.Component<CanvasCellsLayerProps> {
    render() {
        console.log("rendering cells layer");
        console.log(this.props.board);

        return (
            <Layer>
                {
                    this.props.board.cells.map((c, i) =>
                        <CanvasCell
                            key={"cell" + i}
                            cell={c}
                            selectCell={(cell) => this.props.selectCell(cell)}
                        />
                    )
                }
            </Layer>
        );
    }
}