import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { ZoomSlider } from './zoomSlider';
import * as StoreDisplay from '../../store/display';

const mapStateToProps = (state : State) => {
    return {
        currentZoomLevel: state.display.boardZoomLevel
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        changeZoomLevel: (level : number) => dispatch(StoreDisplay.Actions.boardZoom(level))
    };
}

const BoardZoomSlider = connect(mapStateToProps, mapDispatchToProps)(ZoomSlider);
export default BoardZoomSlider;