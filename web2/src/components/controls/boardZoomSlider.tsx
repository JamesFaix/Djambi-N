import * as React from 'react';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as Actions from '../../store/actions';
import CanvasTransformService from '../../viewModel/board/canvasTransformService';

interface BoardZoomSliderProps {
    currentZoomLevel : number,
    changeZoomLevel : (level : number) => void
}

class boardZoomSlider extends React.Component<BoardZoomSliderProps> {
    render() : JSX.Element {
        const level = this.props.currentZoomLevel;
        const cts = CanvasTransformService;
        const scale = cts.getZoomScaleFactor(level);
        return (
            <div>
                <input
                    type="range"
                    value={level}
                    min={cts.minZoomLevel()}
                    max={cts.maxZoomLevel()}
                    onChange={e => this.onSliderChanged(e)}
                />
                {`Zoom ${scale * 100}%`}
            </div>
        );
    }

    private onSliderChanged(e : React.ChangeEvent<HTMLInputElement>) {
        const newLevel = Number(e.target.value);
        this.props.changeZoomLevel(newLevel);
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        currentZoomLevel: state.display.zoomLevel
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        changeZoomLevel: (level : number) => dispatch(Actions.zoom(level))
    };
}

const BoardZoomSlider = connect(mapStateToProps, mapDispatchToProps)(boardZoomSlider);
export default BoardZoomSlider;