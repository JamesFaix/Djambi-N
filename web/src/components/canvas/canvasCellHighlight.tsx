import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

export interface CanvasCellHighlightProps {
    cell : CellView,
    selectCell : (cell : CellView) => void,
    theme : Theme
}

export default class CanvasCelHighlight extends React.Component<CanvasCellHighlightProps> {
    render() {
        const cell = this.props.cell;
        const color = this.props.theme.colors.cells.selectableColor;

        return (
            <CanvasPolygon
                polygon={cell.polygon}
                onClick={() => this.props.selectCell(cell)}
                style={{
                    fillColor: color
                }}
            />
        );
    }
}