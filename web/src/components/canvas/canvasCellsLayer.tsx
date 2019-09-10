import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCell from './canvasCell';
import { BoardView, CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';
import { PieceKind, Game } from '../../api/model';
import { AnimationFrame, BoardTooltipState } from './model';
import { Animation } from 'konva';

interface Props {
    gameId : number,
    board : BoardView,
    theme : Theme,
    selectCell : (cell : CellView) => void,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    scale : number,
    setTooltip : (state : BoardTooltipState) => void,
    game : Game,
    showBoardTooltip : boolean
}

interface State {
    highlightOpacity : number
}

export default class CanvasCellsLayer extends React.Component<Props, State> {
    constructor(props : Props) {
        super(props);
        this.state = { highlightOpacity: 0 };
    }

    componentDidMount(){
        const period = 0.5; //sec
        const maxOpactiy = 0.5;

        const a = new Animation((frame: AnimationFrame) => {
            const timeSec = frame.time / 1000;
            const opacity = Math.abs(Math.sin(timeSec/period)) * maxOpactiy;
            this.setState({highlightOpacity: opacity});
        });

        a.start();
    }

    render() {
        const pieceSize = this.props.scale / this.props.board.cellCountPerSide / 2;

        return (
            <Layer>
                {this.props.board.cells.map((c, i) =>
                    <CanvasCell
                        key={i}
                        cell={c}
                        theme={this.props.theme}
                        highlightOpacity={c.isSelectable ? this.state.highlightOpacity : 0}
                        selectCell={cell => this.props.selectCell(cell)}
                        pieceSize={pieceSize}
                        pieceImage={c.piece ? this.props.pieceImages.get(c.piece.kind) : null}
                        setTooltip={data => this.props.setTooltip(data)}
                        game={this.props.game}
                        showBoardTooltip={this.props.showBoardTooltip}
                    />
                )}
            </Layer>
        );
    }
}