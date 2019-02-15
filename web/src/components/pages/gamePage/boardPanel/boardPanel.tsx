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
    zoomScaleFactor : number,
    windowSizeScaleFactor : number
}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        const zoomLevel = 0;
        this.state = {
            zoomLevel: zoomLevel,
            zoomScaleFactor: this.getZoomScaleFactorFromLevel(zoomLevel),
            windowSizeScaleFactor: this.getWindowSizeScaleFactor(),
        };
    }

    private getScale() : number {
        return this.state.windowSizeScaleFactor
            * this.state.zoomScaleFactor;
    }

    private getTransformedBoard() : BoardView {
        const Transform = Geometry.Transform;
        const bv = this.props.boardView;

        //BoardViews are created with the centroid at (0,0)
        //Offset so none of the board has negative coordinates, since canvases start at (0,0)
        const centroidOffset = this.getCentroidOffsetFromCanvas(bv.regionCount, bv.cellCountPerSide);
        const centroidOffsetTransform = Transform.translate(centroidOffset.x, centroidOffset.y);

        //Magnify the board based on screen size, zoom setting, and board type
        const scale = this.getScale();
        const scaleTransform = Transform.scale(scale, scale);

        //Add a margin for the outline of the board and some whitespace around it within the canvas
        const margin = this.props.boardStrokeWidth + this.props.boardMargin;
        const marginOffsetTransform = Transform.translate(margin, margin);

        //Order is very important. Last transform gets applied to image first
        const t = Transform.compose([
            marginOffsetTransform,
            scaleTransform,
            centroidOffsetTransform
        ]);
        return Geometry.Board.transform(bv, t);
    }

    private getBoardTypeScaleFactor(regionCount : number) : number {
        return 1 / Geometry.RegularPolygon.sideToHeightRatio(regionCount);
    }

    private getZoomScaleFactorFromLevel(zoomLevel : number) : number {
        switch (zoomLevel) {
            case -3: return 0.25;
            case -2: return 0.50;
            case -1: return 0.75;
            case  0: return 1.00;
            case  1: return 1.25;
            case  2: return 1.50;
            case  3: return 2.00;
            case  4: return 3.00;
            case  5: return 4.00;
            default: throw "Unsupported zoom level: " + zoomLevel;
        }
    }

    private getWindowSizeScaleFactor() : number {
        const bv = this.props.boardView;
        const usableSize = this.getUsableSize();
        const maxScale = Geometry.RegularPolygon.largestScaleWithinBox(bv.regionCount, usableSize);
        return maxScale / bv.cellCountPerSide;
    }

    private getUsableSize() : Point {
        const totalMargin = (2 * (this.props.boardMargin + this.props.boardStrokeWidth))
        return Geometry.Point.addScalar(this.props.size, -totalMargin);
    }

    private getCentroidOffsetFromCanvas(regionCount : number, cellCountPerSide : number) : Point {
        return {
            x: Geometry.RegularPolygon.sideToCentroidDistanceFromLeftRatio(regionCount) * cellCountPerSide,
            y: Geometry.RegularPolygon.sideToCentroidDistanceFromTopRatio(regionCount) * cellCountPerSide
        };
    }

    ///--- EVENTS ---

    private onZoomSliderChanged(e : React.ChangeEvent<HTMLInputElement>) : void {
        const level = Number(e.target.value);
        const scaleFactor = this.getZoomScaleFactorFromLevel(level);
        this.setState({
            zoomLevel: level,
            zoomScaleFactor: scaleFactor
        });
    }

    //--- RENDERING ---

    render() {
        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);

        const board = this.getTransformedBoard();
        const margin = 2 * (this.props.boardStrokeWidth + this.props.boardMargin);
        let size = Geometry.Board.size(board);
        size = Geometry.Point.addScalar(size, margin);

        return (
            <div
                className={Classes.thinBorder}
                style={containerStyle}
            >
                <Scrollbars style={containerStyle}>
                    <CanvasBoard
                        board={board}
                        theme={this.props.theme}
                        selectCell={(cell) => this.props.selectCell(cell)}
                        scale={this.getScale()}
                        boardStrokeWidth={this.props.boardStrokeWidth}
                        size={size}
                    />
                </Scrollbars>
                {this.renderZoomControl()}
            </div>
        );
    }

    private renderZoomControl() {
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
                {this.getZoomDescription()}
            </div>
        );
    }

    private getZoomDescription() : string {
        const percent = this.state.zoomScaleFactor * 100;
        return percent + "%";
    }
}