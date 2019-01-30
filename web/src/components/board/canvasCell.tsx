import * as React from 'react';
import { Group } from 'react-konva';
import { CellView } from '../../boardRendering/model';
import CanvasPolygon from './canvasPolygon';
import ThemeService from '../../themes/themeService';
import Color from '../../boardRendering/color';

export interface CanvasCellProps {
    cell: CellView,
    theme : ThemeService,
    selectCell : (cell : CellView) => void
}

export default class CanvasCell extends React.Component<CanvasCellProps> {

    private getCellColor() : string {
        const baseColor = this.props.theme.getCellBaseColor(this.props.cell.type);
        const highlight = this.props.theme.getCellHighlight(this.props.cell.state);

        if (highlight === null) {
            return baseColor;
        } else {
            const hColor = Color.fromHex(highlight[0]);
            const hIntensity = highlight[1];

            return Color.fromHex(baseColor)
                .lighten(hIntensity)
                .multiply(hColor)
                .toHex();
        }
    }

    render() {
        return (
            <Group>
                {this.props.cell.polygons.map((p, i) =>
                    <CanvasPolygon
                        key={"polygon" + i}
                        polygon={p}
                        fillColor={this.getCellColor()}
                        onClick={() => this.props.selectCell(this.props.cell)}
                    />
                )}
            </Group>
        );
    }
}