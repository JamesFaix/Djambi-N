import * as React from 'react';
import Debug from '../../../../../debug';
import Geometry from '../../../../../boardRendering/geometry';
import { CellView } from '../../../../../boardRendering/model';
import { Kernel as K } from '../../../../../kernel';
import { Text } from 'react-konva';

export interface CanvasLabelProps {
    cell : CellView,
    onClick : (cell : CellView) => void,
    regionCount : number
}

export default class CanvasLabel extends React.Component<CanvasLabelProps> {

    render() {
        if (!Debug.showCellLabels) {
            return undefined;
        }
        const cell = this.props.cell;

        let text = K.copy.getCellLabel(cell.id, this.props.regionCount);
        if (Debug.showPieceAndCellIds && cell.piece !== null) {
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