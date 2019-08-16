import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import Colors from '../../utilities/colors';

export interface CanvasCellProps {
    cell : CellView,
    selectCell : (cell : CellView) => void
}

export default class CanvasCell extends React.Component<CanvasCellProps> {
    render() {
        const cell = this.props.cell;
        const color = Colors.getCellViewColor(cell);
        let borderColor = Colors.getCellViewBorderColor(cell);
        if (!borderColor) {
            borderColor = color;
        }

        return (
            <CanvasPolygon
                polygon={cell.polygon}
                onClick={() => this.props.selectCell(cell)}
                style={{
                    fillColor: color,
                    strokeColor: borderColor,
                    strokeWidth: 1 //Stroke is necessary to fill gaps between polygons belonging to the same cell
                }}
            />
        );
    }
}