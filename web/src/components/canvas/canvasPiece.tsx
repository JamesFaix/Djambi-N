import * as React from 'react';
import { Image } from 'react-konva';
import { PieceView } from '../../viewModel/board/model';
import { Point } from '../../viewModel/board/model';

export interface CanvasPieceProps {
    piece : PieceView,
    size : number,
    location : Point,
    image : HTMLImageElement,
}

export default class CanvasPiece extends React.Component<CanvasPieceProps> {
    render() {
        return (
            <Image
                image={this.props.image}
                x={this.props.location.x}
                y={this.props.location.y}
                height={this.props.size}
                width={this.props.size}
                shadowColor={"black"}
                shadowOpacity={0.5}
                opacity={1}
                shadowBlur={5}
                shadowOffsetX={5}
                shadowOffsetY={5}
            />
        );
    }
}