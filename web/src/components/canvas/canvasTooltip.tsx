import * as React from 'react';
import { Layer, Label, Tag, Text } from 'react-konva';
import { Point } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

interface CanvasTooltipProps {
    visible : boolean,
    text : string,
    position : Point,
    theme : Theme
}

export default class CanvasTooltip extends React.Component<CanvasTooltipProps>{
    render() {
        if (!this.props.visible) {
            return null;
        }

        const colors = this.props.theme.colors;

        return (
            <Layer>
                <Label
                    x={this.props.position.x}
                    y={this.props.position.y}
                    opacity={1}
                >
                    <Tag
                        stroke={colors.border}
                        strokeWidth={1}
                        fill={colors.background}
                        shadowColor={"black"}
                        shadowBlur={5}
                        shadowOpacity={1}
                    />
                    <Text
                        text={this.props.text}
                        fill={colors.text}
                        padding={5}
                    />
                </Label>
            </Layer>
        );
    }
}