import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';

export interface CanvasCellProps {
    cell : CellView,
    theme : Theme
}

export default class CanvasCell extends React.Component<CanvasCellProps> {
    render() {
        const cell = this.props.cell;
        const theme = this.props.theme;
        const color = ThemeService.getCellColor(theme, cell);
        let borderColor = ThemeService.getCellBorderColor(theme, cell.type);
        if (!borderColor) {
            borderColor = color;
        }

        return (
            <CanvasPolygon
                polygon={cell.polygon}
                style={{
                    fillColor: color,
                    strokeColor: borderColor,
                    strokeWidth: 1 //Stroke is necessary to fill gaps between polygons belonging to the same cell
                }}
            />
        );
    }
}