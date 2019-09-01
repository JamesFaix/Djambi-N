import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { ZoomSlider } from './zoomSlider';
import * as StoreDisplay from '../../store/display';
import Geometry from '../../viewModel/board/geometry';
import { Point } from '../../viewModel/board/model';
import * as React from 'react';

interface BoardZoomSliderProps {
    zoomLevel : number,
    scrollPercent : Point,
    onZoom : (level:number) => void,
    scrollAreaSize : Point
}

class boardZoomSlider extends React.Component<BoardZoomSliderProps> {
    render() {
        return (
            <ZoomSlider
                level={this.props.zoomLevel}
                changeLevel={level => this.onZoom(level)}
            />
        );
    }

    private onZoom(level : number) {
        const oldLevel = this.props.zoomLevel;
        const newState : any = { zoomLevel : level };

        //If going from a negative level (where the whole board is visible) to a positive one, start zoom focus at center
        if (oldLevel <= 0) {
            newState.scrollPercent = { x: 0.5, y: 0.5 };
        }

        this.props.onZoom(level);

        const {scrollbar} = this.refs as any;

        const scrollContainerSize = { x: scrollbar.getClientWidth(), y: scrollbar.getClientHeight() };
        const scrollableAreaSize = Geometry.Point.subtract(this.props.scrollAreaSize, scrollContainerSize);

        const scrollPercent = this.props.scrollPercent;
        const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

        scrollbar.scrollLeft(scrollPosition.x);
        scrollbar.scrollTop(scrollPosition.y);
    }
}

const mapStateToProps = (state : State) => {
    return {
        zoomLevel: state.display.boardZoomLevel,
        scrollPercent: state.display.boardScrollPercent,
        scrollAreaSize: state.display.boardContainerSize
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onZoom: (level : number) => dispatch(StoreDisplay.Actions.boardZoom(level))
    };
}

const BoardZoomSlider = connect(mapStateToProps, mapDispatchToProps)(boardZoomSlider);
export default BoardZoomSlider;