import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import BoardGeometry from '../../../../boardRendering/boardGeometry';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';

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
        const boardView = this.props.boardView;
        //const canvasSize = BoardGeometry.boardDiameter(boardView) + "px";
        const canvasStyle = Styles.combine([
            Styles.noMargin,
            Styles.width(this.props.width),
            Styles.height(this.props.height)
        ]);

        return (
            <div
                className={Classes.thinBorder}
                style={canvasStyle}
            >
                <CanvasBoard
                    board={boardView}
                    theme={this.props.theme}
                    selectCell={(cell) => this.props.selectCell(cell)}
                />
                <button onClick={_ => this.props.updateCellSize(this.props.boardView.cellSize * 2)}>
                    +
                </button>
                <button onClick={_ => this.props.updateCellSize(this.props.boardView.cellSize / 2)}>
                    -
                </button>

            </div>
        );
    }
}