import * as React from 'react';
import { Group } from 'react-konva';
import { CellView } from '../../boardRendering/model';
import CanvasPolygon from './canvasPolygon';
import ThemeService from '../../themes/themeService';

export interface CanvasCellProps {
    cell: CellView,
    theme : ThemeService,
    selectCell : (cell : CellView) => void
}

export default class CanvasCell extends React.Component<CanvasCellProps> {

    render() {
        const color = this.props.theme.getCellColor(this.props.cell.type, this.props.cell.state);

        return (
            <Group>
                {
                    this.props.cell.polygons.map((p, i) =>
                        <CanvasPolygon
                            key={"polygon" + i}
                            polygon={p}
                            fillColor={color}
                            onClick={() => this.props.selectCell(this.props.cell)}
                        />
                    )
                }
            </Group>
        );
    }
}