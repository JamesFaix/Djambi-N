import * as React from 'react';
import { Image, Text, Label, Tag } from 'react-konva';
import { PieceView } from '../../viewModel/board/model';
import { Point } from '../../viewModel/board/model';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';
import Copy from '../../utilities/copy';
import { Game } from '../../api/model';

export interface CanvasPieceProps {
    piece : PieceView,
    size : number,
    location : Point,
    image : HTMLImageElement,
    theme : Theme,
    game : Game
}

interface CanvasPieceState {
    showTooltip : boolean
}

export default class CanvasPiece extends React.Component<CanvasPieceProps, CanvasPieceState> {
    constructor(props: CanvasPieceProps) {
        super(props);
        this.state = {
            showTooltip: false
        };
    }

    render() {
        const colorId = this.props.piece.colorId;
        const playerColor = colorId ? ThemeService.getPlayerColor(this.props.theme, colorId) : null;
        return (<>
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
                onMouseMove={() => {
                    this.setState({showTooltip: true});
                }}
                onMouseOut={() => {
                    this.setState({showTooltip: false});
                }}
            />
            <Label
                visible={this.state.showTooltip}
                x={this.props.location.x}
                y={this.props.location.y}
                opacity={0.75}
            >
                <Tag
                    fill={this.props.theme.colors.background}
                    shadowColor={"black"}
                    shadowBlur={5}
                    shadowOpacity={0.5}
                />
                <Text
                    text={Copy.getPieceViewLabel(this.props.piece, this.props.game)}
                    fill={this.props.theme.colors.text}
                />
            </Label>
        </>);
    }
}