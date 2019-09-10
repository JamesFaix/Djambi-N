import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import ThemeService from '../../themes/themeService';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

const CanvasCellBackgroundLayer : React.SFC<{
    cell : CellView,
    theme : Theme
}> = props => {
    const color = ThemeService.getCellColor(props.theme, props.cell);
    let borderColor = ThemeService.getCellBorderColor(props.theme, props.cell.type);
    if (!borderColor) {
        borderColor = color;
    }

    return (
        <CanvasPolygon
            polygon={props.cell.polygon}
            style={{
                fillColor: color,
                strokeColor: borderColor,
                strokeWidth: 1 //Stroke is necessary to fill gaps between polygons belonging to the same cell
            }}
        />
    );
}
export default CanvasCellBackgroundLayer;