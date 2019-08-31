import * as React from 'react';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import Geometry from '../../viewModel/board/geometry';
import { Point } from '../../viewModel/board/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as StoreDisplay from '../../store/display';

interface BoardScrollAreaProps {
    onScroll : (scrollPercent : Point) => void,
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

    private onScroll(e : positionValues) : void {
        const scrollContainerSize = { x: e.clientWidth, y: e.clientHeight };
        const scrollContentSize = { x: e.scrollWidth, y: e.scrollHeight };
        const scrollableAreaSize = Geometry.Point.subtract(scrollContentSize, scrollContainerSize);

        const scrollPosition = { x: e.scrollLeft, y: e.scrollTop };
        const scrollPercent = Geometry.Point.divideSafe(scrollPosition, scrollableAreaSize);

        this.props.onScroll(scrollPercent);
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onScroll : (scrollPercent : Point) => dispatch(StoreDisplay.Actions.boardScroll(scrollPercent)),
    };
}

const BoardScrollArea = connect(null, mapDispatchToProps)(boardScrollArea);
export default BoardScrollArea;