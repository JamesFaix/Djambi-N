import * as React from 'react';
import { Group } from 'react-konva';
import { CellView, CellState } from '../../boardRendering/model';
import CanvasPolygon from './canvasPolygon';
import ThemeService from '../../themes/themeService';
import Color from '../../boardRendering/color';

export interface CanvasCellProps {
    cell: CellView,
    theme : ThemeService
}

export default class CanvasCell extends React.Component<CanvasCellProps> {

    render() {
        return (
            <Group>
                {
                    this.props.cell.polygons.map((p, i) =>
                        <CanvasPolygon
                            key={"polygon" + i}
                            polygon={p}
                            fillColor={this.getCellColor(this.props.cell)}
                        />
                    )
                }
            </Group>
        );
    }

    private getCellColor(cell : CellView) : string {
        const color = this.props.theme.getCellColor(cell.type);

        switch (cell.state)
        {
            case CellState.Default:
                return color;

            case CellState.Selected:
                return Color.fromHex(color)
                    .lighten(0.75)
                    .multiply(Color.greenHighlight())
                    .toHex();

            case CellState.Selectable:
                return Color.fromHex(color)
                    .lighten(0.50)
                    .multiply(Color.yellowHighlight())
                    .toHex();
        }
    }
}