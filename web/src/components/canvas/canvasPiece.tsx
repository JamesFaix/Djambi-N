import * as React from 'react';
import { Image } from 'react-konva';
import { PieceView } from '../../viewModel/board/model';
import { Point } from '../../viewModel/board/model';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';

export interface CanvasPieceProps {
    piece : PieceView,
    onClick : () => void,
    size : number,
    location : Point,
    image : HTMLImageElement,
    theme : Theme
}

export default class CanvasPiece extends React.Component<CanvasPieceProps> {
    render() {
        const colorId = this.props.piece.colorId;
        const playerColor = colorId ? ThemeService.getPlayerColor(this.props.theme, colorId) : null;
        return (
            <Image
                image={this.props.image}
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