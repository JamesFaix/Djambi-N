import * as React from 'react';
import { PieceView } from '../../boardRendering/model';
import ThemeService from '../../themes/themeService';
import { Image } from 'react-konva';
import { Point } from '../../geometry/model';

export interface CanvasPieceProps {
    piece : PieceView,
    theme : ThemeService,
    onClick : () => void,
    size : number,
    location : Point
}

export interface CanvasPieceState {
    image : HTMLImageElement
}

export default class CanvasPiece extends React.Component<CanvasPieceProps, CanvasPieceState> {
    constructor(props : CanvasPieceProps) {
        super(props);
        this.state = {
            image : null
        };
    }

    componentDidMount() {
        const image = new (window as any).Image();
        image.src = this.props.theme.getPieceImage(this.props.piece.kind);
        image.onload = () => {
            this.setState({image : image})
        };
    }

    render() {
        return (
            <Image
               image={this.state.image}
               x={this.props.location.x}
               y={this.props.location.y}
               height={this.props.size}
               width={this.props.size}
               shadowColor={this.props.theme.getPlayerColor(this.props.piece.colorId)}
               shadowOpacity={1}
               shadowBlur={50}
               shadowOffsetX={5}
               shadowOffsetY={5}
            />
        );
    }
}