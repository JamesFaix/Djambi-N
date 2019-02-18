import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';
import Scrollbars from 'react-custom-scrollbars';
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
    zoomLevel : number
}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        this.state = {
            zoomLevel: 0
        };
    }

    ///--- EVENTS ---

    private onZoomSliderChanged(e : React.ChangeEvent<HTMLInputElement>) : void {
        const level = Number(e.target.value);
        this.setState({
            zoomLevel: level
        });
    }

    //--- RENDERING ---

    render() {
        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);

        const p = this.props;

        const cts = new CanvasTransformService(
            p.size,
            p.boardMargin,
            p.boardStrokeWidth,
            this.state.zoomLevel,
            p.boardView.regionCount
        );

        const board = Geometry.Board.transform(p.boardView, cts.getBoardViewTransform());

        return (
            <div
                className={Classes.thinBorder}
                style={containerStyle}
            >
                <Scrollbars style={containerStyle}>
                    <CanvasBoard
                        board={board}
                        theme={p.theme}
                        selectCell={(cell) => p.selectCell(cell)}
                        scale={cts.getScale()}
                        boardStrokeWidth={p.boardStrokeWidth}
                        size={cts.getBoardSizeWithScaleButNoZoom()}
                    />
                </Scrollbars>
                {this.renderZoomControl(cts.getZoomScaleFactor())}
            </div>
        );
    }

    private renderZoomControl(scaleFactor : number) {
        return (
            <div>
                Zoom
                <input
                    type={InputTypes.Range}
                    onChange={e => this.onZoomSliderChanged(e)}
                    value={this.state.zoomLevel}
                    min={-3}
                    max={5}
                />
                {(scaleFactor * 100) + "%"}
            </div>
        );
    }
}