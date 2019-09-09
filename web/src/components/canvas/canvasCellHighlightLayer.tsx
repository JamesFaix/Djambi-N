import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

interface CanvasCellHighlightLayerProps {
    cell : CellView,
    theme : Theme,
    opacity : number
}

export default class CanvasCellHighlightLayer extends React.Component<CanvasCellHighlightLayerProps> {
    render() {
        if (!this.props.cell.isSelectable) {
            return null;
        }

        return (
            <CanvasPolygon
                polygon={this.props.cell.polygon}
                style={{
                    fillColor: this.props.theme.colors.cells.selectableColor,
                    opacity: this.props.opacity
                }}
            />
        );
    }
}