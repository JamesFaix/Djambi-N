import React, { FC } from 'react';
import { Layer, Label, Tag, Text } from 'react-konva';
import { Point } from '../../board/model';
import { colors } from './styles';

interface Props {
  visible : boolean,
  text : string,
  position : Point,
}

const CanvasTooltip : FC<Props> = ({ visible, text, position }) => {
  if (!visible) {
    return null;
  }

  return (
    <Layer>
      <Label
        x={position.x}
        y={position.y}
        opacity={1}
      >
      <Tag
        stroke={colors.tooltipBorder}
        strokeWidth={1}
        fill={colors.tooltipBackground}
        shadowColor={"black"}
        shadowBlur={5}
        shadowOpacity={1}
      />
      <Text
        text={text}
        fill={colors.tooltipText}
        padding={5}
      />
      </Label>
    </Layer>
  );
}
export default CanvasTooltip;
