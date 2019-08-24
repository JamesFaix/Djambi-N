import * as React from 'react';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import Geometry from '../../viewModel/board/geometry';
import { ZoomSlider } from './zoomSlider';
import { Point } from '../../viewModel/board/model';

interface ZoomableScrollAreaProps {
    size : Point,
    innerStyle : React.CSSProperties,
    outerStyle : React.CSSProperties,
    zoomLevel : number,
    scrollPercent : Point,
    onScroll : (scrollPercent : Point) => void,
    onZoom : (level : number) => void
}

export default class ZoomableScrollArea extends React.Component<ZoomableScrollAreaProps> {
    render() {
        return (
            <div style={this.props.outerStyle}>
                <Scrollbars
                    ref='scrollbar'
                    onScrollFrame={e => this.onScroll(e)}
                    style={this.props.innerStyle}
                >
                        {this.props.children}
                </Scrollbars>
                <ZoomSlider
                    level={this.props.zoomLevel}
                    changeLevel={level => this.onZoom(level)}
                />
            </div>
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
        const scrollableAreaSize = Geometry.Point.subtract(this.props.size, scrollContainerSize);

        const scrollPercent = this.props.scrollPercent;
        const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

        scrollbar.scrollLeft(scrollPosition.x);
        scrollbar.scrollTop(scrollPosition.y);
    }
}