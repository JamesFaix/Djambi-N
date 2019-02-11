import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';
import Scrollbars from 'react-custom-scrollbars';
import { InputTypes } from '../../../../constants';
import Geometry from '../../../../geometry/geometry';
import BoardGeometry from '../../../../boardRendering/boardGeometry';
import { Point } from '../../../../geometry/model';

export interface BoardPanelProps {
    game : Game,
    theme : ThemeService,
    boardView : BoardView,
    selectCell : (cell : CellView) => void,
    height : string,
    width : string,
    boardMargin : number,
    boardStrokeWidth :number
}

export interface BoardPanelState {
    zoomLevel : number
    zoomMultiplier : number,
    windowSizeMultiplier : number,
    boardTypeMultiplier : number
}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        const zoomLevel = 0;
        this.state = {
            zoomLevel: zoomLevel,
            zoomMultiplier: this.getZoomMultiplierFromZoomLevel(zoomLevel),
            windowSizeMultiplier: this.getWindowSizeMultiplier(),
            boardTypeMultiplier: this.getBoardTypeMultiplier(props.game.parameters.regionCount)
        };
    }

    private getMagnification() : number {
        return this.state.windowSizeMultiplier
            * this.state.zoomMultiplier
            * this.state.boardTypeMultiplier;
    }

    private getMagnifiedBoard() : BoardView {
        let bv = this.props.boardView;

        //BoardViews are created with a side facing up, but we want a side facing down.
        //Rotate 180 degrees to fix
        const rotationTransform = Geometry.transformRotation(360 / bv.regionCount / 2);

        //BoardViews are created with the centroid at (0,0)
        //Offset so none of the board has negative coordinates, since canvases start at (0,0)
        const centroidOffset = this.getCentroidOffsetFromCanvas(bv.regionCount, bv.cellCountPerSide);
        const centroidOffsetTransform = Geometry.transformTranslate(centroidOffset.x, centroidOffset.y);

        //Magnify the board based on screen size, zoom setting, and board type
        const mag = this.getMagnification();
        const scaleTransform = Geometry.transformScale(mag, mag);

        //Add a margin for the outline of the board and some whitespace around it within the canvas
        const margin = this.props.boardStrokeWidth + this.props.boardMargin;
        const marginOffsetTransform = Geometry.transformTranslate(margin, margin);

        //Order is very important. Last transform gets applied to image first
        let t = Geometry.transformCompose([
            marginOffsetTransform,
            scaleTransform,
            centroidOffsetTransform,
            rotationTransform
        ]);
        return BoardGeometry.boardTransform(bv, t);
    }

    private getBoardTypeMultiplier(regionCount : number) : number {
        //These numbers are based off the relative heights of the shapes, when sitting with an edge down
        switch (regionCount) {
            case 3: return 1.155;
            case 4: return 1.000;
            case 5: return 0.650;
            case 6: return 0.577;
            case 7: return 0.456;
            case 8: return 0.414;
            default: throw "Unsupported region count.";
        }

        //This formula approximates the trend: e^(-0.2 * regionCount) * 2
    }

    private getZoomMultiplierFromZoomLevel(zoomLevel : number) : number {
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

    private getWindowSizeMultiplier() : number {
        return 50; //TODO: Make this change based on window or container size later
    }

    private getCentroidOffsetFromCanvas(regionCount : number, cellCountPerSide : number) : Point {
        let x : number;
        let y : number;

        //These numbers all assume a side length of 1, and an edge is facing down
        switch (regionCount) {
            case 3:
                x = 0.500; // 1/2 width
                y = 0.577; // radius
                break;

            case 4:
                x = 0.500; // 1/2 width (also apothem)
                y = 0.500; // 1/2 width (also apothem)
                break;

            case 5:
                x = 0.810; // 1/2 width
                y = 0.850; // radius
                break;

            case 6:
                x = 1.000; // 1/2 width (also radius)
                y = 0.866; // apothem
                break;

            case 7:
                x = 1.125; // 1/2 width
                y = 1.152; // radius
                break;

            case 8:
                x = 1.205; // 1/2 width (also apothem)
                y = 1.205; // 1/2 width (also apothem)
                break;

            default:
                throw "Unsupported region count.";
        }

        //Enlarge for the actual side length
        return {
            x: x * cellCountPerSide,
            y: y * cellCountPerSide
        };
    }

    ///--- EVENTS ---

    private onZoomSliderChanged(e : React.ChangeEvent<HTMLInputElement>) : void {
        const level = Number(e.target.value);
        const multiplier = this.getZoomMultiplierFromZoomLevel(level);
        this.setState({
            zoomLevel: level,
            zoomMultiplier: multiplier
        });
    }

    //--- RENDERING ---

    render() {
        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);
        const board = this.getMagnifiedBoard();
        const dimensions = BoardGeometry.boardDimensions(board);
        const margin = 2 * (this.props.boardStrokeWidth + this.props.boardMargin);
        const width = dimensions.x + margin;
        const height = dimensions.y + margin;

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
                        magnification={this.getMagnification()}
                        boardStrokeWidth={this.props.boardStrokeWidth}
                        width={width}
                        height={height}
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
        const percent = this.state.zoomMultiplier * 100;
        return percent + "%";
    }
}