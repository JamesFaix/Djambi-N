import * as React from 'react';
import { PieceView } from '../../../../../boardRendering/model';
import { Image } from 'react-konva';
import { Point } from '../../../../../boardRendering/model';
import { Kernel as K } from '../../../../../kernel';

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
        console.log("CanvasPiece.render Id:" + this.props.piece.id + ", Kind: " + this.props.piece.kind);

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