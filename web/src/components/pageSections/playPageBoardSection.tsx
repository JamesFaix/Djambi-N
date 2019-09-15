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
import * as StoreDisplay from '../../store/display';
import { ZoomSlider } from '../controls/zoomSlider';
import { Dispatch } from 'redux';
import { Point } from '../../viewModel/board/model';

export const BoardSection : React.SFC<{}> = _ => {
    const scrollbarsRef : React.MutableRefObject<Scrollbars> = React.useRef(null);
    const game = Selectors.game();
    const board = useSelector((state : AppState) => state.activeGame.boardView);
    const display = useSelector((state : AppState) => state.display);
    const debugSettings = useSelector((state : AppState) => state.settings.debug);
    const dispatch = useDispatch();

    React.useEffect(() => {
        window.removeEventListener("resize", null);
        window.addEventListener("resize", () => {
            const scrollbars = scrollbarsRef.current;
            if (!scrollbars) { return; }
            const size : Point = {
                x: scrollbars.getClientWidth(),
                y: scrollbars.getClientHeight()
            };
            Controller.Display.resizeBoardArea(size);
        });
    });

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
                ref={scrollbarsRef}
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
                changeLevel={level => onZoom(level, display, scrollbarsRef.current, dispatch)}
            />
        </div>
    );
}

function onScroll(e : positionValues, dispatch : Dispatch) {
    const scrollContainerSize = { x: e.clientWidth, y: e.clientHeight };
    const scrollContentSize = { x: e.scrollWidth, y: e.scrollHeight };
    const scrollPosition = { x: e.scrollLeft, y: e.scrollTop };
    const scrollableAreaSize = Geometry.Point.subtract(scrollContentSize, scrollContainerSize);
    const scrollPercent = Geometry.Point.divideSafe(scrollPosition, scrollableAreaSize);
    dispatch(StoreDisplay.Actions.boardScroll(scrollPercent));
}

function onZoom(level : number, display : StoreDisplay.State, scrollbars : Scrollbars, dispatch : Dispatch) {
    const oldLevel = display.boardZoomLevel;
    const newState : any = { zoomLevel : level };

    //If going from a negative level (where the whole board is visible) to a positive one, start zoom focus at center
    if (oldLevel <= 0) {
        newState.scrollPercent = { x: 0.5, y: 0.5 };
    }

    dispatch(StoreDisplay.Actions.boardZoom(level));

    const scrollContainerSize = { x: scrollbars.getClientWidth(), y: scrollbars.getClientHeight() };
    const scrollableAreaSize = Geometry.Point.subtract(display.boardContainerSize, scrollContainerSize);

    const scrollPercent = display.boardScrollPercent;
    const scrollPosition = Geometry.Point.multiply(scrollPercent, scrollableAreaSize);

    scrollbars.scrollLeft(scrollPosition.x);
    scrollbars.scrollTop(scrollPosition.y);
}