import * as React from 'react';
import { Stage, Layer } from 'react-konva';
import { BoardView } from '../../boardRendering/model';
import ThemeService from '../../themes/themeService';
import CanvasCell from './canvasCell';

export interface CanvasBoardProps {
    board : BoardView,
    theme : ThemeService
}

export default class CanvasBoard extends React.Component<CanvasBoardProps> {

    render() {
        return (
            <Stage width={1000} height={1000}>
                <Layer>
                    {
                        this.props.board.cells.map((c, i) =>
                            <CanvasCell
                                key={"cell" + i}
                                cell={c}
                                theme={this.props.theme}
                            />
                        )
                    }
                </Layer>
            </Stage>
        );
    }
}