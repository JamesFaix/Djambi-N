import * as React from 'react';
import { Image } from 'react-konva';
import { PieceView } from '../../viewModel/board/model';
import { Point } from '../../viewModel/board/model';
import Colors from '../../utilities/colors';
import Images from '../../utilities/images';

export interface CanvasPieceProps {
    piece : PieceView,
    onClick : () => void,
    size : number,
    location : Point
}

export default class CanvasPiece extends React.Component<CanvasPieceProps> {
    render() {
        const playerColor = Colors.getColorFromPlayerColorId(this.props.piece.colorId);
        return (
            <Image
                image={Images.getPieceImage(this.props.piece.kind)}
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