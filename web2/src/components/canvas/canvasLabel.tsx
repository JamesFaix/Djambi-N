import * as React from 'react';
import Geometry from '../../viewModel/board/geometry';
import { CellView, BoardView } from '../../viewModel/board/model';
import { Text } from 'react-konva';
import * as Copy from '../../utilities/copy';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';

export interface CanvasLabelProps {
    board : BoardView,
    cell : CellView,
    onClick : () => void,
    theme : Theme,
    debugSettings : DebugSettings
}

export default class CanvasLabel extends React.Component<CanvasLabelProps> {

    render() {
        const s = this.props.debugSettings;

        if (!s.showCellLabels) {
            return null;
        }
        const cell = this.props.cell;

        let text = Copy.getCellViewLabel(this.props.theme, cell.id, this.props.board, s.showCellAndPieceIds);
        if (s.showCellAndPieceIds && cell.piece !== null) {
            text += "\nP " + cell.piece.id;
        }

        const color = ThemeService.getCellTextColor(this.props.theme, cell.type);

        const rect = Geometry.Cell.boundingBox(cell);

        return (
            <Text
                x={rect.left}
                y={rect.top}
                height={rect.bottom - rect.top}
                width={rect.right - rect.left}
                text={text}
                align={"center"}
                verticalAlign={"middle"}
                fill={color}
                shadowColor='#000000'
                shadowBlur={10}
                shadowOpacity={1}
                onClick={() => this.props.onClick()}
            />
        );
    }
}