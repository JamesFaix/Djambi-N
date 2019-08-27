import * as React from 'react';
import CanvasBoard, { CanvasBoardStyle } from '../canvas/canvasBoard';
import { BoardView, CellView } from '../../viewModel/board/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import CanvasTransformService, { CanvasTranformData } from '../../viewModel/board/canvasTransformService';
import { PieceKind } from '../../api/model';
import * as Images from '../../utilities/images';
import BoardArea from './boardArea';
import ApiActions from '../../apiActions';

export interface BoardSectionProps {
    gameId : number,
    board : BoardView,
    transformData : CanvasTranformData,
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
        if (!this.props.board) {
            return null;
        }

        const internalSize = CanvasTransformService.getSize(this.props.transformData);

        const boardStyle : CanvasBoardStyle = {
            width: internalSize.x,
            height: internalSize.y,
            strokeWidth: 5, //5 maybe put in settings somewhere
            strokeColor: "black", //pull from Colors module
            scale: CanvasTransformService.getScale(this.props.transformData)
        };

        return (
            <div>
                <BoardArea
                    innerStyle={{
                        width:"1000px",
                        height:"1000px"
                    }}
                    outerStyle={{
                        width:"100%",
                        height:"100%",
                        display:"flex",
                        flexDirection:"column",
                        alignItems:"center"
                    }}
                >
                    <CanvasBoard
                        style={boardStyle}
                        gameId={this.props.gameId}
                        board={this.props.board}
                        selectCell={cell => this.props.selectCell(this.props.gameId, cell)}
                        pieceImages={this.props.pieceImages}
                    />
                </BoardArea>
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    if (!state.activeGame.game) {
        return null;
    }

    const game = state.activeGame.game;
    return {
        gameId: game ? game.id : null,
        board: state.activeGame.boardView,
        zoomLevel: state.display.boardZoomLevel,
        pieceImages: state.images.pieces,
        transformData: {
            containerSize: state.display.boardContainerSize,
            canvasMargin: state.display.canvasMargin,
            contentPadding: state.display.canvasContentPadding,
            zoomLevel: state.display.boardZoomLevel,
            regionCount: state.activeGame.game.parameters.regionCount
        }
    };
}

const mapDispatchToProps = (dispatch: Dispatch) => {
    return {
        selectCell: (gameId : number, cell : CellView) => ApiActions.selectCell(gameId, cell.id)(dispatch),
        loadPieceImages: () => Images.init(dispatch)
    };
}

const BoardSection = connect(mapStateToProps, mapDispatchToProps)(boardSection);
export default BoardSection;