import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

const CanvasCellHighlightLayer : React.SFC<{
    cell : CellView,
    theme : Theme,
    opacity : number
}> = props => {
    if (!props.cell.isSelectable) {
        return null;
    }

    return (
        <CanvasPolygon
            polygon={props.cell.polygon}
            style={{
                fillColor: props.theme.colors.cells.selectableColor,
                opacity: props.opacity
            }}
        />
    );
}
export default CanvasCellHighlightLayer