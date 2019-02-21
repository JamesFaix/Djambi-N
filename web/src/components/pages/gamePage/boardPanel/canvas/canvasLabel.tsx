import * as React from 'react';
import { CellView } from '../../../../../boardRendering/model';
import { Text } from 'react-konva';
import Debug from '../../../../../debug';
import Geometry from '../../../../../boardRendering/geometry';
import CopyService from '../../../../../copyService';

export interface CanvasLabelProps {
    cell : CellView,
    onClick : (cell : CellView) => void
}

export default class CanvasLabel extends React.Component<CanvasLabelProps> {

    render() {
        if (!Debug.showPieceAndCellIds) {
            return undefined;
        }
        const cell = this.props.cell;

        let text = CopyService.locationToString(cell.locations[0]);
        text += "\nC " + cell.id;
        if (cell.piece !== null) {
            text += "\nP " + cell.piece.id;
        }

        const rect = Geometry.Cell.rectangle(cell);

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
                onClick={() => this.props.onClick(this.props.cell)}
            />
        );
    }
}