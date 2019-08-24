import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as Actions from '../../store/actions';
import { ZoomSlider } from './zoomSlider';

const mapStateToProps = (state : AppState) => {
    return {
        currentZoomLevel: state.display.boardZoomLevel
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        changeZoomLevel: (level : number) => dispatch(Actions.boardZoom(level))
    };
}

const BoardZoomSlider = connect(mapStateToProps, mapDispatchToProps)(ZoomSlider);
export default BoardZoomSlider;