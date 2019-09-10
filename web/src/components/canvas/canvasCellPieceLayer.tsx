import * as React from 'react';
import { Image } from 'react-konva';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';
import { CellView } from '../../viewModel/board/model';
import Geometry from '../../viewModel/board/geometry';

const CanvasCellPieceLayer : React.SFC<{
    size : number,
    image : HTMLImageElement,
    theme : Theme,
    cell : CellView
}> = props => {
    const c = props.cell;

    if (!c.piece) {
        return null;
    }

    const colorId = c.piece.colorId;
    const playerColor = colorId ? ThemeService.getPlayerColor(props.theme, colorId) : null;

    const cellCenter = Geometry.Cell.centroid(c);
    const offset = { x: -(props.size/2), y: -(props.size/2) };
    const pieceLocation = Geometry.Point.add(cellCenter, offset);

    return (
        <Image
            image={props.image}
            x={pieceLocation.x}
            y={pieceLocation.y}
            height={props.size}
            width={props.size}
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
export default CanvasCellPieceLayer;