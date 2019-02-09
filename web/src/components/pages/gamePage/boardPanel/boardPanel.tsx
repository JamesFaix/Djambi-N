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

export interface BoardPanelProps {
    game : Game,
    theme : ThemeService,
    boardView : BoardView,
    selectCell : (cell : CellView) => void,
    height : string,
    width : string
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
        const mag = this.getMagnification();
        const transform = Geometry.transformScale(mag, mag);
        return BoardGeometry.boardTransform(this.props.boardView, transform);
    }

    private getBoardTypeMultiplier(regionCount : number) : number {
        //Through trial an error, I found that this formula keeps boards
        //of varying regionCount about the same absolute size
        return Math.pow(Math.E, (-0.2 * regionCount)) * 2;
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

    private onZoomSliderChanged(e : React.ChangeEvent<HTMLInputElement>) : void {
        const level = Number(e.target.value);
        const multiplier = this.getZoomMultiplierFromZoomLevel(level);
        this.setState({
            zoomLevel: level,
            zoomMultiplier: multiplier
        });
    }

    private getZoomDescription() : string {
        const percent = this.state.zoomMultiplier * 100;
        return percent + "%";
    }

    render() {
        const containerStyle = Styles.combine([
            Styles.width("100%"),
            Styles.height("100%")
        ]);

        return (
            <div
                className={Classes.thinBorder}
                style={containerStyle}
            >
                {this.renderZoomControl()}
                <Scrollbars style={containerStyle}>
                    <CanvasBoard
                        board={this.getMagnifiedBoard()}
                        theme={this.props.theme}
                        selectCell={(cell) => this.props.selectCell(cell)}
                        magnification={this.getMagnification()}
                    />
                </Scrollbars>
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
}