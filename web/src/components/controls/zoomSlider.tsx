import * as React from 'react';
import CanvasTransformService from '../../viewModel/board/canvasTransformService';
import HtmlInputTypes from '../htmlInputTypes';

interface ZoomSliderProps {
    level : number,
    changeLevel : (level : number) => void
}

export class ZoomSlider extends React.Component<ZoomSliderProps> {
    render() : JSX.Element {
        const level = this.props.level;
        const cts = CanvasTransformService;
        const scale = cts.getZoomScaleFactor(level);
        return (
            <div
                id="zoom-slider"
            >
                <input
                    type={HtmlInputTypes.Range}
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
        this.props.changeLevel(newLevel);
    }
}