import * as React from 'react';
import CanvasBoard, { CanvasBoardStyle } from '../canvas/canvasBoard';
import { BoardView, CellView } from '../../viewModel/board/model';
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import CanvasTransformService, { CanvasTranformData } from '../../viewModel/board/canvasTransformService';
import { PieceKind } from '../../api/model';
import ApiActions from '../../apiActions';
import { Classes } from '../../styles/styles';
import BoardScrollArea from './boardScrollArea';
import BoardZoomSlider from '../controls/boardZoomSlider';
import { Theme } from '../../themes/model';

export interface BoardSectionProps {
    gameId : number,
    board : BoardView,
    transformData : CanvasTranformData,
    zoomLevel : number,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    selectCell : (gameId: number, cell : CellView) => void,
    theme : Theme
}

class boardSection extends React.Component<BoardSectionProps> {
    render(){
        if (!this.props.board) {
            return null;
        }

        const internalSize = CanvasTransformService.getSize(this.props.transformData);

        const boardStyle : CanvasBoardStyle = {
            width: internalSize.x,
            height: internalSize.y,
            strokeWidth: 5, //TODO: maybe put in settings somewhere
            scale: CanvasTransformService.getScale(this.props.transformData),
            theme: this.props.theme
        };

        return (
            <div
                id="board-section"
                className={Classes.boardSection}
            >
                <BoardScrollArea>
                    <CanvasBoard
                        style={boardStyle}
                        gameId={this.props.gameId}
                        board={this.props.board}
                        selectCell={cell => this.props.selectCell(this.props.gameId, cell)}
                        pieceImages={this.props.pieceImages}
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

    const game = state.activeGame.game;
    return {
        gameId: game ? game.id : null,
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
        theme: state.display.theme
    };
}

const mapDispatchToProps = (dispatch: Dispatch) => {
    return {
        selectCell: (gameId : number, cell : CellView) => ApiActions.selectCell(gameId, cell.id)(dispatch)
    };
}

const BoardSection = connect(mapStateToProps, mapDispatchToProps)(boardSection);
export default BoardSection;