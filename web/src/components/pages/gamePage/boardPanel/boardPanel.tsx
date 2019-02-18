import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import { InputTypes } from '../../../../constants';
import Geometry from '../../../../boardRendering/geometry';
import { Point } from '../../../../boardRendering/model';
import CanvasTransformService from '../../../../boardRendering/canvasTransformService';

export interface BoardPanelProps {
    game : Game,
    theme : ThemeService,
    boardView : BoardView,
    selectCell : (cell : CellView) => void,
    size : Point,
    boardMargin : number,
    boardStrokeWidth :number
}

export interface BoardPanelState {
    zoomLevel : number,
    scrollPercent : Point
}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        this.state = {
            zoomLevel: 0,
            scrollPercent: Geometry.Point.zero()
        };
    }

    private getCanvasTransformService() {
        return new CanvasTransformService(
            this.props.size,
            this.props.boardMargin,
            this.props.boardStrokeWidth,
            this.state.zoomLevel,
            this.props.boardView.regionCount
        );
    }

    ///--- EVENTS ---

    private onZoomSliderChanged(e : React.ChangeEvent<HTMLInputElement>) : void {
        const level = Number(e.target.value);

        this.setState({
            zoomLevel: level
        }, () => {
            const {scrollbar} = this.refs as any;
            const cts = this.getCanvasTransformService();

            const scrollContainerSize = { x: scrollbar.getClientWidth(), y: scrollbar.getClientHeight() };
            const scrollableAreaSize = Geometry.Point.subtract(cts.getSize(), scrollContainerSize);

            const scrollPercent = this.state.scrollPercent;
            const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

            scrollbar.scrollLeft(scrollPosition.x);
            scrollbar.scrollTop(scrollPosition.y);
        });
    }

    private onScroll(e : positionValues) : void {
        const scrollContainerSize = { x: e.clientWidth, y: e.clientHeight };
        const scrollContentSize = { x: e.scrollWidth, y: e.scrollHeight };
        const scrollableAreaSize = Geometry.Point.subtract(scrollContentSize, scrollContainerSize);

        const scrollPosition = { x: e.scrollLeft, y: e.scrollTop };
        const scrollPercent = Geometry.Point.divideSafe(scrollPosition, scrollableAreaSize);

        this.setState({
            scrollPercent: scrollPercent
        });
    }

    //--- RENDERING ---

    render() {
        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);

        const p = this.props;

        const cts = this.getCanvasTransformService();

        const board = Geometry.Board.transform(p.boardView, cts.getBoardViewTransform());

        return (
            <div
                className={Classes.thinBorder}
                style={containerStyle}
            >
                <Scrollbars
                    ref='scrollbar'
                    style={containerStyle}
                    onScrollFrame={e => this.onScroll(e)}
                >
                    <CanvasBoard
                        board={board}
                        theme={p.theme}
                        selectCell={(cell) => p.selectCell(cell)}
                        scale={cts.getScale()}
                        boardStrokeWidth={p.boardStrokeWidth}
                        size={cts.getSize()}
                    />
                </Scrollbars>
                {this.renderZoomControl(cts)}
            </div>
        );
    }

    private renderZoomControl(cts : CanvasTransformService) {
        return (
            <div>
                Zoom
                <input
                    type={InputTypes.Range}
                    onChange={e => this.onZoomSliderChanged(e)}
                    value={this.state.zoomLevel}
                    min={cts.minZoomLevel()}
                    max={cts.maxZoomLevel()}
                />
                {(cts.getZoomScaleFactor() * 100) + "%"}
            </div>
        );
    }
}