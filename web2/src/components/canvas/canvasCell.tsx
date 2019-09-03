import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import ThemeService from '../../themes/themeService';

export interface CanvasCellProps {
    cell : CellView,
    selectCell : (cell : CellView) => void,
    theme : Theme
}

export default class CanvasCell extends React.Component<CanvasCellProps> {
    render() {
        const cell = this.props.cell;
        const theme = this.props.theme;
        const color = ThemeService.getCellColor(theme, cell);
        let borderColor = theme.colors.cells.border;
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