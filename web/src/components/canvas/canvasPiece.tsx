import * as React from 'react';
import { Image } from 'react-konva';
import { Kernel as K } from '../../kernel';
import { PieceView } from '../../boardRendering/model';
import { Point } from '../../boardRendering/model';

export interface CanvasPieceProps {
    piece : PieceView,
    onClick : () => void,
    size : number,
    location : Point
}

export default class CanvasPiece extends React.Component<CanvasPieceProps> {
    constructor(props : CanvasPieceProps) {
        super(props);
    }

    render() {
        const playerColor = K.theme.getPlayerColor(this.props.piece.colorId);
        return (
            <Image
               image={K.theme.getPieceImage(this.props.piece.kind)}
               x={this.props.location.x}
               y={this.props.location.y}
               height={this.props.size}
               width={this.props.size}
               shadowColor={playerColor}
               //Fade neutral pieces
               shadowOpacity={playerColor ? 1 : 0}
               opacity={playerColor ? 1 : 0.75}
               shadowBlur={30}
               shadowOffsetX={5}
               shadowOffsetY={5}
               onClick={() => this.props.onClick()}
            />
        );
    }
}