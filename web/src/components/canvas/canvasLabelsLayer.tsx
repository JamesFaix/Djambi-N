import * as React from 'react';
import { Layer } from 'react-konva';
import { BoardView } from '../../viewModel/board/model';
import CanvasLabel from './canvasLabel';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';

export interface CanvasLabelsLayerProps {
    gameId : number,
    board : BoardView,
    theme : Theme,
    debugSettings : DebugSettings
}

export default class CanvasLabelsLayer extends React.Component<CanvasLabelsLayerProps> {
    render() {
        const p = this.props;
        if (!p.debugSettings.showCellLabels){
            return null;
        }

        return (
            <Layer>
                {p.board.cells.map((c, i) =>
                    <CanvasLabel
                        key={i}
                        board={p.board}
                        cell={c}
                        theme={p.theme}
                        debugSettings={p.debugSettings}
                    />
                )}
            </Layer>
        );
    }
}