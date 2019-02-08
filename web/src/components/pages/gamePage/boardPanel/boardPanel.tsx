import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';
import Scrollbars from 'react-custom-scrollbars';

export interface BoardPanelProps {
    game : Game,
    theme : ThemeService,
    boardView : BoardView,
    selectCell : (cell : CellView) => void,
    height : string,
    width : string,
    updateCellSize : (cellSize : number) => void
}

export interface BoardPanelState {

}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        this.state = {

        };
    }

    render() {
        const p = this.props;
        const bv = p.boardView;

        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);

        return (
            <div
                className={Classes.thinBorder}
                style={containerStyle}
            >
                <Scrollbars style={containerStyle}>
                    <CanvasBoard
                        board={bv}
                        theme={p.theme}
                        selectCell={(cell) => p.selectCell(cell)}
                    />
                </Scrollbars>
                <button onClick={_ => p.updateCellSize(bv.cellSize * 2)}>
                    +
                </button>
                <button onClick={_ => p.updateCellSize(bv.cellSize / 2)}>
                    -
                </button>

            </div>
        );
    }
}