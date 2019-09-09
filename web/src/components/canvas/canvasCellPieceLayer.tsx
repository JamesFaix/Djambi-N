import * as React from 'react';
import { Image } from 'react-konva';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';
import { CellView, Point } from '../../viewModel/board/model';
import Geometry from '../../viewModel/board/geometry';

interface CanvasCellPieceLayerProps {
    size : number,
    image : HTMLImageElement,
    theme : Theme,
    cell : CellView
}

export default class CanvasCellPieceLayer extends React.Component<CanvasCellPieceLayerProps> {
    render() {
        const c = this.props.cell;

        if (!c.piece) {
            return null;
        }

        const colorId = c.piece.colorId;
        const playerColor = colorId ? ThemeService.getPlayerColor(this.props.theme, colorId) : null;
        const pieceLocation = this.getPieceLocation(c);

        return (
            <Image
                image={this.props.image}
                x={pieceLocation.x}
                y={pieceLocation.y}
                height={this.props.size}
                width={this.props.size}
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
        const size = this.props.size;
        const cellCenter = Geometry.Cell.centroid(cell);
        const offset = { x: -(size/2), y: -(size/2) };
        return Geometry.Point.add(cellCenter, offset);
    }
}