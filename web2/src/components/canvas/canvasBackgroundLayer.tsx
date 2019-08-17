import * as React from 'react';
import { Layer } from 'react-konva';
import { BoardView } from '../../viewModel/board/model';
import CanvasPolygon, { CanvasPolygonStyle } from './canvasPolygon';
import { connect } from 'react-redux';
import { AppState } from '../../store/state';

export interface CanvasBackgroundLayerProps {
    board : BoardView,
    style : CanvasPolygonStyle
}

class canvasBackgroundLayer extends React.Component<CanvasBackgroundLayerProps> {
    render() {
        return (
            <Layer>
                <CanvasPolygon
                    polygon={this.props.board.polygon}
                    style={this.props.style}
                />
            </Layer>
        );
    }
}


const mapStateToProps = (state : AppState) => {
    return {
        board: state.activeGame.boardView
    };
};

const CanvasBackgroundLayer = connect(mapStateToProps)(canvasBackgroundLayer);
export default CanvasBackgroundLayer;