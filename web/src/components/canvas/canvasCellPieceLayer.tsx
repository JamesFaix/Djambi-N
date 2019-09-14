import * as React from 'react';
import { Image } from 'react-konva';
import { CellView } from '../../viewModel/board/model';
import Geometry from '../../viewModel/board/geometry';

const CanvasCellPieceLayer : React.SFC<{
    size : number,
    image : HTMLImageElement,
    cell : CellView
}> = props => {
    const c = props.cell;

    if (!c.piece) {
        return null;
    }

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
            shadowColor={"black"}
            shadowOpacity={0.5}
            opacity={1}
            shadowBlur={5}
            shadowOffsetX={5}
            shadowOffsetY={5}
        />
    );
}
export default CanvasCellPieceLayer;