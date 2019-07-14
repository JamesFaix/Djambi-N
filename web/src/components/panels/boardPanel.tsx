import * as React from 'react';
import CanvasBoard from '../canvas/canvasBoard';
import CanvasTransformService from '../../boardRendering/canvasTransformService';
import Geometry from '../../boardRendering/geometry';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import { BoardView, CellView } from '../../boardRendering/model';
import { Game } from '../../api/model';
import { InputTypes } from '../../constants';
import { Kernel as K } from '../../kernel';
import { Point } from '../../boardRendering/model';

export interface BoardPanelProps {
    game : Game,
    boardView : BoardView,
    selectCell : (cell : CellView) => void,
    size : Point,
    boardMargin : number,
    boardStrokeWidth : number
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
        const oldLevel = this.state.zoomLevel;
        const newLevel = Number(e.target.value);

        const newState : any = { zoomLevel : newLevel };

        //If going from a negative level (where the whole board is visible) to a positive one, start zoom focus at center
        if (oldLevel <= 0) {
            newState.scrollPercent = { x: 0.5, y: 0.5 };
        }

        this.setState(newState, () => {
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
        const containerStyle = K.styles.combine([
            K.styles.width("100%"),
            K.styles.height("100%")
        ]);

        const p = this.props;

        const cts = this.getCanvasTransformService();

        const board = Geometry.Board.transform(p.boardView, cts.getBoardViewTransform());

        return (
            <div
                className={K.classes.thinBorder}
                style={containerStyle}
            >
                <Scrollbars
                    ref='scrollbar'
                    style={containerStyle}
                    onScrollFrame={e => this.onScroll(e)}
                >
                    <CanvasBoard
                        board={board}
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