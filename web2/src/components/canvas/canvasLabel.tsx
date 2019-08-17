import * as React from 'react';
import Debug from '../../debug';
import Geometry from '../../viewModel/board/geometry';
import { CellView, BoardView } from '../../viewModel/board/model';
import { Text } from 'react-konva';
import * as Copy from '../../utilities/copy';

export interface CanvasLabelProps {
    board : BoardView,
    cell : CellView,
    onClick : () => void,
}

export default class CanvasLabel extends React.Component<CanvasLabelProps> {

    render() {
        if (!Debug.showCellLabels) {
            return null;
        }
        const cell = this.props.cell;

        let text = Copy.getCellViewLabel(cell.id, this.props.board);
        if (Debug.showPieceAndCellIds && cell.piece !== null) {
            text += "\nP " + cell.piece.id;
        }

        const rect = Geometry.Cell.boundingBox(cell);

        return (
            <Text
                x={rect.left}
                y={rect.top}
                height={rect.bottom - rect.top}
                width={rect.right - rect.left}
                text={text}
                align={"center"}
                verticalAlign={"middle"}
                fill='#FFFFFF'
                shadowColor='#000000'
                shadowBlur={10}
                shadowOpacity={1}
                onClick={() => this.props.onClick()}
            />
        );
    }
}