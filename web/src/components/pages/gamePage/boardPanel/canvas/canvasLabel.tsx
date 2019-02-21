import * as React from 'react';
import { CellView } from '../../../../../boardRendering/model';
import { Text } from 'react-konva';
import Debug from '../../../../../debug';
import Geometry from '../../../../../boardRendering/geometry';

export interface CanvasLabelProps {
    cell : CellView
}

export default class CanvasLabel extends React.Component<CanvasLabelProps> {

    render() {
        if (!Debug.showPieceAndCellIds) {
            return undefined;
        }
        const cell = this.props.cell;
        const location = Geometry.Cell.centroid(cell);

        let text = "C " + cell.id;
        if (cell.piece !== null) {
            text += "\nP " + cell.piece.id;
        }

        return (
            <Text
                x={location.x}
                y={location.y}
                text={text}
                fill='#FFFFFF'
                shadowColor='#000000'
                shadowBlur={10}
                shadowOpacity={1}
            />
        );
    }
}