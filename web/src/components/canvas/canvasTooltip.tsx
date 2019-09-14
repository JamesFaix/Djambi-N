import * as React from 'react';
import { Layer, Label, Tag, Text } from 'react-konva';
import { Point } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';

const CanvasTooltip : React.SFC<{
    visible : boolean,
    text : string,
    position : Point,
    theme : Theme
}> = props => {
    if (!props.visible) {
        return null;
    }

    const colors = props.theme.colors;

    return (
        <Layer>
            <Label
                x={props.position.x}
                y={props.position.y}
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
                    text={props.text}
                    fill={colors.text}
                    padding={5}
                />
            </Label>
        </Layer>
    );
}
export default CanvasTooltip;