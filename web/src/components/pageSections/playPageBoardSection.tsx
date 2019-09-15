import * as React from 'react';
import CanvasBoard from '../canvas/canvasBoard';
import { State as AppState, CustomAction } from '../../store/root';
import { useSelector, useDispatch } from 'react-redux';
import CanvasTransformService, { CanvasTranformData } from '../../viewModel/board/canvasTransformService';
import { Classes } from '../../styles/styles';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import Scrollbars, { positionValues } from 'react-custom-scrollbars';
import Geometry from '../../viewModel/board/geometry';
import * as StoreDisplay from '../../store/display';
import { ZoomSlider } from '../controls/zoomSlider';
import { Dispatch } from 'redux';

export const BoardSection : React.SFC<{}> = _ => {
    const scrollbars = React.useRef(null);
    const game = Selectors.game();
    const board = useSelector((state : AppState) => state.activeGame.boardView);
    const display = useSelector((state : AppState) => state.display);
    const debugSettings = useSelector((state : AppState) => state.settings.debug);
    const dispatch = useDispatch();
  //  React.useEffect(() => onLoad(display, dispatch));

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
            <Scrollbars
                ref={scrollbars}
                onScrollFrame={e => onScroll(e, dispatch)}
                id="board-scroll-area"
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
            </Scrollbars>
            <ZoomSlider
                level={display.boardZoomLevel}
                changeLevel={level => onZoom(level, display, scrollbars, dispatch)}
            />
        </div>
    );
}

function onLoad(display : StoreDisplay.State, dispatch : Dispatch) {
    dispatch(StoreDisplay.Actions.boardScroll(display.boardScrollPercent));
    dispatch(StoreDisplay.Actions.boardZoom(display.boardZoomLevel));
}

function onScroll(e : positionValues, dispatch : Dispatch) {
    const scrollContainerSize = { x: e.clientWidth, y: e.clientHeight };
    const scrollContentSize = { x: e.scrollWidth, y: e.scrollHeight };
    const scrollPosition = { x: e.scrollLeft, y: e.scrollTop };
    const scrollableAreaSize = Geometry.Point.subtract(scrollContentSize, scrollContainerSize);
    const scrollPercent = Geometry.Point.divideSafe(scrollPosition, scrollableAreaSize);
    dispatch(StoreDisplay.Actions.boardScroll(scrollPercent));
}

function onZoom(level : number, display : StoreDisplay.State, scrollbars : any, dispatch : Dispatch) {
    const oldLevel = display.boardZoomLevel;
    const newState : any = { zoomLevel : level };

    //If going from a negative level (where the whole board is visible) to a positive one, start zoom focus at center
    if (oldLevel <= 0) {
        newState.scrollPercent = { x: 0.5, y: 0.5 };
    }

    dispatch(StoreDisplay.Actions.boardZoom(level));

    const sb = scrollbars.current;

    const scrollContainerSize = { x: sb.getClientWidth(), y: sb.getClientHeight() };
    const scrollableAreaSize = Geometry.Point.subtract(display.boardContainerSize, scrollContainerSize);

    const scrollPercent = display.boardScrollPercent;
    const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

    sb.scrollLeft(scrollPosition.x);
    sb.scrollTop(scrollPosition.y);
}