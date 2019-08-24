import { Dispatch } from "redux";
import { AppState } from "../../store/state";
import { connect } from "react-redux";
import ZoomableScrollArea from "../controls/zoomableScrollArea";
import * as Actions from '../../store/actions';
import { Point } from "../../viewModel/board/model";

const mapStateToProps = (state : AppState) => {
    return {
        size: state.display.boardContainerSize,
        zoomLevel: state.display.boardZoomLevel,
        scrollPercent: state.display.boardScrollPercent
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onScroll : (scrollPercent : Point) => dispatch(Actions.boardScroll(scrollPercent)),
        onZoom : (level : number) => dispatch(Actions.boardZoom(level))
    };
}

const BoardArea = connect(mapStateToProps, mapDispatchToProps)(ZoomableScrollArea);
export default BoardArea;