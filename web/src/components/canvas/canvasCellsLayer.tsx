import * as React from 'react';
import { Layer } from 'react-konva';
import CanvasCell from './canvasCell';
import { BoardView, CellView } from '../../viewModel/board/model';
import { Theme } from '../../themes/model';
import { PieceKind, Game } from '../../api/model';
import { BoardTooltipData } from './canvasBoard';

export interface CanvasCellsLayerProps {
    gameId : number,
    board : BoardView,
    theme : Theme,
    highlightOpacity : number,
    selectCell : (cell : CellView) => void,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    scale : number,
    setTooltip : (data : BoardTooltipData) => void,
    game : Game
}

export default class CanvasCellsLayer extends React.Component<CanvasCellsLayerProps> {
    render() {
        const pieceSize = this.props.scale / this.props.board.cellCountPerSide / 2;

        return (
            <Layer>
                {this.props.board.cells.map((c, i) =>
                    <CanvasCell
                        key={i}
                        cell={c}
                        theme={this.props.theme}
                        highlightOpacity={c.isSelectable ? this.props.highlightOpacity : 0}
                        selectCell={cell => this.props.selectCell(cell)}
                        pieceSize={pieceSize}
                        pieceImage={c.piece ? this.props.pieceImages.get(c.piece.kind) : null}
                        setTooltip={data => this.props.setTooltip(data)}
                        game={this.props.game}
                    />
                )}
            </Layer>
        );
    }
}