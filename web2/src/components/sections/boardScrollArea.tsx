import * as React from 'react';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import Geometry from '../../viewModel/board/geometry';
import { Point } from '../../viewModel/board/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as StoreDisplay from '../../store/display';
import { State } from '../../store/root';

interface BoardScrollAreaProps {
    scrollPercent : Point,
    onScroll : (scrollPercent : Point) => void,
    onResize : (size : Point) => void
}

class boardScrollArea extends React.Component<BoardScrollAreaProps> {
    render() {
        return (
            <Scrollbars
                ref='scrollbar'
                onScrollFrame={e => this.onScroll(e)}
                id="board-scroll-area"
            >
                {this.props.children}
            </Scrollbars>
        );
    }

    componentDidMount(){
        const r = this.refs.scrollbar as any;
        const size = { x: r.getClientWidth(), y: r.getClientHeight() };
        this.props.onResize(size);
        this.props.onScroll(this.props.scrollPercent)
    }

    private onScroll(e : positionValues) : void {
        const scrollContainerSize = { x: e.clientWidth, y: e.clientHeight };
        const scrollContentSize = { x: e.scrollWidth, y: e.scrollHeight };
        const scrollableAreaSize = Geometry.Point.subtract(scrollContentSize, scrollContainerSize);

        const scrollPosition = { x: e.scrollLeft, y: e.scrollTop };
        const scrollPercent = Geometry.Point.divideSafe(scrollPosition, scrollableAreaSize);

        this.props.onScroll(scrollPercent);
    }
}

const mapStateToProps = (state : State) => {
    return {
        scrollPercent: state.display.boardScrollPercent
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onScroll : (scrollPercent : Point) => dispatch(StoreDisplay.Actions.boardScroll(scrollPercent)),
        onResize : (size : Point) => dispatch(StoreDisplay.Actions.boardAreaResize(size))
    };
}

const BoardScrollArea = connect(mapStateToProps, mapDispatchToProps)(boardScrollArea);
export default BoardScrollArea;