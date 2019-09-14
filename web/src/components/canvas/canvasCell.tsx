import * as React from 'react';
import { CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';
import { Group } from 'react-konva';
import Copy from '../../utilities/copy';
import { KonvaEventObject } from 'konva';
import { Game } from '../../api/model';
import CanvasCellPieceLayer from './canvasCellPieceLayer';
import CanvasCellHighlightLayer from './canvasCellHighlightLayer';
import CanvasCellBackgroundLayer from './canvasCellBackgroundLayer';
import { BoardTooltipState } from './model';

export default class CanvasCell extends React.Component< {
    cell : CellView,
    theme : Theme,
    highlightOpacity : number,
    selectCell : (cell : CellView) => void,
    pieceImage : HTMLImageElement,
    pieceSize : number,
    showBoardTooltip : boolean,
    setTooltip : (state : BoardTooltipState) => void,
    game : Game
}> {
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
                    size={this.props.pieceSize}
                    image={this.props.pieceImage}
                />
            </Group>
        );
    }

    private updateTooltip(e : KonvaEventObject<MouseEvent>) {
        if (!this.props.showBoardTooltip) {
            return;
        }

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