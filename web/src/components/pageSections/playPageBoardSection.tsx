import * as React from 'react';
import CanvasBoard from '../canvas/canvasBoard';
import { State as AppState } from '../../store/root';
import { useSelector, useDispatch } from 'react-redux';
import CanvasTransformService, { CanvasTranformData } from '../../viewModel/board/canvasTransformService';
import { Classes } from '../../styles/styles';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import Geometry from '../../viewModel/board/geometry';
import { Point } from '../../viewModel/board/model';
import * as StoreDisplay from '../../store/display';
import { ZoomSlider } from '../controls/zoomSlider';

export const BoardSection : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const board = useSelector((state : AppState) => state.activeGame.boardView);
    const display = useSelector((state : AppState) => state.display);
    const debugSettings = useSelector((state : AppState) => state.settings.debug);
    const dispatch = useDispatch();

    //React will blow up if you don't call the same hooks in the same order during each render
    //so no early termination until all hooks are used
    if (!game || !board) { return null; }

    const transformData : CanvasTranformData = {
        containerSize: display.boardContainerSize,
        canvasMargin: display.canvasMargin,
        contentPadding: display.canvasContentPadding,
        zoomLevel: display.boardZoomLevel,
        regionCount: game.parameters.regionCount
    };
    const internalSize = CanvasTransformService.getSize(transformData);

    return (
        <div
            id="board-section"
            className={Classes.boardSection}
        >
            <BoardScrollArea
                scrollPercent={display.boardScrollPercent}
                onScroll={scrollPercent => dispatch(StoreDisplay.Actions.boardScroll(scrollPercent))}
                onResize={size => dispatch(StoreDisplay.Actions.boardAreaResize(size))}
            >
                <CanvasBoard
                    style={{
                        width: internalSize.x,
                        height: internalSize.y,
                        strokeWidth: 5, //TODO: maybe put in settings somewhere
                        scale: CanvasTransformService.getScale(transformData),
                        theme: display.theme
                    }}
                    game={game}
                    board={board}
                    selectCell={cell => Controller.Game.selectCell(game.id, cell.id)}
                    pieceImages={display.images.pieces}
                    debugSettings={debugSettings}
                />
            </BoardScrollArea>
            <BoardZoomSlider
                zoomLevel={display.boardZoomLevel}
                scrollPercent={display.boardScrollPercent}
                onZoom={level => dispatch(StoreDisplay.Actions.boardZoom(level))}
                scrollAreaSize={display.boardContainerSize}
            />
        </div>
    );
}

class BoardScrollArea extends React.Component<{
    scrollPercent : Point,
    onScroll : (scrollPercent : Point) => void,
    onResize : (size : Point) => void
}> {
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

    componentDidMount(){
        const r = this.refs.scrollbar as any;
        const size = { x: r.getClientWidth(), y: r.getClientHeight() };
        this.props.onResize(size);
        this.props.onScroll(this.props.scrollPercent)
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

class BoardZoomSlider extends React.Component<{
    zoomLevel : number,
    scrollPercent : Point,
    onZoom : (level:number) => void,
    scrollAreaSize : Point
}> {
    render() {
        return (
            <ZoomSlider
                level={this.props.zoomLevel}
                changeLevel={level => this.onZoom(level)}
            />
        );
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
        const scrollableAreaSize = Geometry.Point.subtract(this.props.scrollAreaSize, scrollContainerSize);

        const scrollPercent = this.props.scrollPercent;
        const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

        scrollbar.scrollLeft(scrollPosition.x);
        scrollbar.scrollTop(scrollPosition.y);
    }
}