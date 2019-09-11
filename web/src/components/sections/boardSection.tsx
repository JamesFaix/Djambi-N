import * as React from 'react';
import CanvasBoard from '../canvas/canvasBoard';
import { BoardView, CellView } from '../../viewModel/board/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import CanvasTransformService, { CanvasTranformData } from '../../viewModel/board/canvasTransformService';
import { PieceKind, Game } from '../../api/model';
import { Classes } from '../../styles/styles';
import BoardScrollArea from './boardScrollArea';
import BoardZoomSlider from '../controls/boardZoomSlider';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';
import Controller from '../../controller';

export interface BoardSectionProps {
    game : Game,
    board : BoardView,
    transformData : CanvasTranformData,
    zoomLevel : number,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    selectCell : (gameId: number, cell : CellView) => void,
    theme : Theme,
    debugSettings : DebugSettings
}

class boardSection extends React.Component<BoardSectionProps> {
    render(){
        const p = this.props;
        if (!p.board) {
            return null;
        }

        const internalSize = CanvasTransformService.getSize(p.transformData);

        const boardStyle = {
            width: internalSize.x,
            height: internalSize.y,
            strokeWidth: 5, //TODO: maybe put in settings somewhere
            scale: CanvasTransformService.getScale(p.transformData),
            theme: p.theme
        };

        return (
            <div
                id="board-section"
                className={Classes.boardSection}
            >
                <BoardScrollArea>
                    <CanvasBoard
                        style={boardStyle}
                        game={p.game}
                        board={p.board}
                        selectCell={cell => p.selectCell(p.game.id, cell)}
                        pieceImages={p.pieceImages}
                        debugSettings={this.props.debugSettings}
                    />
                </BoardScrollArea>
                <BoardZoomSlider/>
            </div>
        );
    }
}

const mapStateToProps = (state : State) => {
    if (!state.activeGame.game) {
        return null;
    }

    return {
        game: state.activeGame.game,
        board: state.activeGame.boardView,
        zoomLevel: state.display.boardZoomLevel,
        pieceImages: state.display.images.pieces,
        transformData: {
            containerSize: state.display.boardContainerSize,
            canvasMargin: state.display.canvasMargin,
            contentPadding: state.display.canvasContentPadding,
            zoomLevel: state.display.boardZoomLevel,
            regionCount: state.activeGame.game.parameters.regionCount
        },
        theme: state.display.theme,
        debugSettings: state.settings.debug,
        selectCell: (gameId : number, cell : CellView) => Controller.Game.selectCell(gameId, cell.id)
    };
}

const BoardSection = connect(mapStateToProps)(boardSection);
export default BoardSection;