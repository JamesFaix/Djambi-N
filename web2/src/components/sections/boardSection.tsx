import * as React from 'react';
import CanvasBoard, { CanvasBoardStyle } from '../canvas/canvasBoard';
import { BoardView, CellView } from '../../viewModel/board/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import CanvasTransformService from '../../viewModel/board/canvasTransformService';
import { PieceKind } from '../../api/model';
import * as Images from '../../utilities/images';

export interface BoardSectionProps {
    gameId : number,
    board : BoardView,
    zoomLevel : number,
    pieceImages : Map<PieceKind, HTMLImageElement>,
    selectCell : (gameId: number, cell : CellView) => void,
    loadPieceImages : () => void
}

class boardSection extends React.Component<BoardSectionProps> {
    componentDidMount(){
        this.props.loadPieceImages();
    }

    render(){
        const zoomScale = CanvasTransformService.getZoomScaleFactor(this.props.zoomLevel);
        const sizeScale = 500;
        const scale = zoomScale * sizeScale;

        const boardStyle : CanvasBoardStyle = {
            width: 1000, //get from CanvasTransformService
            height : 1000, //get from CanvasTransformService
            strokeWidth : 5, //5 maybe put in settings somewhere
            strokeColor : "black", //pull from Colors module
            scale : scale //get from CanvasTransformService
        };

        if (!this.props.board) {
            return null;
        }

        return (
            <div>
                <CanvasBoard
                    style={boardStyle}
                    gameId={this.props.gameId}
                    board={this.props.board}
                    selectCell={cell => this.props.selectCell(this.props.gameId, cell)}
                    pieceImages={this.props.pieceImages}
                />
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    const game = state.activeGame.game;
    return {
        gameId: game ? game.id : null,
        board: state.activeGame.boardView,
        zoomLevel: state.display.zoomLevel,
        pieceImages: state.images.pieces
    };
}

const mapDispatchToProps = (dispatch: Dispatch) => {
    return {
        selectCell: (gameId : number, cell : CellView) => ThunkActions.selectCell(gameId, cell.id)(dispatch),
        loadPieceImages: () => Images.init(dispatch)
    };
}

const BoardSection = connect(mapStateToProps, mapDispatchToProps)(boardSection);
export default BoardSection;