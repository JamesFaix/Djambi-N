import * as React from 'react';
import CanvasPolygon from './canvasPolygon';
import { CellView, Point } from '../../viewModel/board/model';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';
import { Image, Group } from 'react-konva';
import Geometry from '../../viewModel/board/geometry';
import Copy from '../../utilities/copy';
import { BoardTooltipData } from './canvasBoard';
import { KonvaEventObject } from 'konva';
import { Game } from '../../api/model';

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
        return (
            <Group
                onMouseMove={e => this.updateTooltip(e)}
                onClick={() => this.onClick()}
            >
                {this.renderBackground()}
                {this.renderHighlight()}
                {this.renderPiece()}
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

    private renderBackground() {
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

    private renderHighlight() {
        const c = this.props.cell;
        if (!c.isSelectable) {
            return null;
        }

        return (
            <CanvasPolygon
                polygon={c.polygon}
                onClick={() => this.props.selectCell(c)}
                style={{
                    fillColor: this.props.theme.colors.cells.selectableColor,
                    opacity: this.props.highlightOpacity
                }}
            />
        );
    }

    private renderPiece() {
        const c = this.props.cell;
        if (!c.piece) {
            return null;
        }

        const colorId = c.piece.colorId;
        const playerColor = colorId ? ThemeService.getPlayerColor(this.props.theme, colorId) : null;
        const pieceLocation = this.getPieceLocation(c);

        return (
            <Image
                image={this.props.pieceImage}
                x={pieceLocation.x}
                y={pieceLocation.y}
                height={this.props.pieceSize}
                width={this.props.pieceSize}
                shadowColor={playerColor}
                //Fade neutral pieces
                shadowOpacity={playerColor ? 1 : 0}
                opacity={playerColor ? 1 : 0.75}
                shadowBlur={30}
                shadowOffsetX={5}
                shadowOffsetY={5}
            />
        );
    }

    private getPieceLocation(cell : CellView) : Point {
        const size = this.props.pieceSize;
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.add(cellCenter, offset);
    }
}