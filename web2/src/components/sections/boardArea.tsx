import { Dispatch } from "redux";
import { State } from "../../store/root";
import { connect } from "react-redux";
import ZoomableScrollArea from "../controls/zoomableScrollArea";
import { Point } from "../../viewModel/board/model";
import * as StoreDisplay from '../../store/display';

const mapStateToProps = (state : State) => {
    return {
        size: state.display.boardContainerSize,
        zoomLevel: state.display.boardZoomLevel,
        scrollPercent: state.display.boardScrollPercent
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onScroll : (scrollPercent : Point) => dispatch(StoreDisplay.Actions.boardScroll(scrollPercent)),
        onZoom : (level : number) => dispatch(StoreDisplay.Actions.boardZoom(level))
    };
}

const BoardArea = connect(mapStateToProps, mapDispatchToProps)(ZoomableScrollArea);
export default BoardArea;