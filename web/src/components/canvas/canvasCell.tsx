import * as React from 'react';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';
import { Group } from 'react-konva';
import Copy from '../../utilities/copy';
import { BoardTooltipData } from './canvasBoard';
import { KonvaEventObject } from 'konva';
import { Game } from '../../api/model';
import CanvasCellPieceLayer from './canvasCellPieceLayer';
import CanvasCellHighlightLayer from './canvasCellHighlightLayer';
import CanvasCellBackgroundLayer from './canvasCellBackgrounLayer';

export interface CanvasCellProps {
    cell : CellView,
    theme : Theme,
    highlightOpacity : number,
    selectCell : (cell : CellView) => void,
    pieceImage : HTMLImageElement,
    pieceSize : number,
    setTooltip : (data : BoardTooltipData) => void,
    game : Game
}

export default class CanvasCell extends React.Component<CanvasCellProps> {
    render() {
        const c = this.props.cell;
        const th = this.props.theme;

        return (
            <Group
                onMouseMove={e => this.updateTooltip(e)}
                onClick={() => this.onClick()}
            >
                <CanvasCellBackgroundLayer
                    cell={c}
                    theme={th}
                />
                <CanvasCellHighlightLayer
                    cell={c}
                    theme={th}
                    opacity={this.props.highlightOpacity}
                />
                <CanvasCellPieceLayer
                    cell={c}
                    theme={th}
                    size={this.props.pieceSize}
                    image={this.props.pieceImage}
                />
            </Group>
        );
    }

    private updateTooltip(e : KonvaEventObject<MouseEvent>) {
        const offset = 5;

        const pos = {
            x: e.evt.offsetX + offset,
            y: e.evt.offsetY + offset
        };

        const c = this.props.cell;
        let text = Copy.getCellViewLabel(c);
        if (c.piece) {
            text += "\n" + Copy.getPieceViewLabel(c.piece, this.props.game);
        }

        const data = {
            visible: true,
            text: text,
            position: pos
        };

        this.props.setTooltip(data);
    }

    private onClick() {
        if (this.props.cell.isSelectable) {
            this.props.selectCell(this.props.cell);
        }
    }
}