import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import ThemeService from '../../themes/themeService';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

interface CanvasCellBackgroundLayerProps {
    cell : CellView,
    theme : Theme
}

export default class CanvasCellBackgroundLayer extends React.Component<CanvasCellBackgroundLayerProps> {
    render() {
        const c = this.props.cell;
        const theme = this.props.theme;
        const color = ThemeService.getCellColor(theme, c);
        let borderColor = ThemeService.getCellBorderColor(theme, c.type);
        if (!borderColor) {
            borderColor = color;
        }

        return (
            <CanvasPolygon
                polygon={c.polygon}
                style={{
                    fillColor: color,
                    strokeColor: borderColor,
                    strokeWidth: 1 //Stroke is necessary to fill gaps between polygons belonging to the same cell
                }}
            />
        );
    }
}