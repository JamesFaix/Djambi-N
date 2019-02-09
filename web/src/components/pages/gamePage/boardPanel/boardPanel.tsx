import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import { BoardView, CellView } from '../../../../boardRendering/model';
import CanvasBoard from './canvas/canvasBoard';
import Scrollbars from 'react-custom-scrollbars';
import LabeledInput from '../../../controls/labeledInput';
import { InputTypes } from '../../../../constants';

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
        this.state = {
            zoomLevel: 0,
            zoomMultiplier: 1,
            windowSizeMultiplier: 50, //TODO: Make this change based on window or container size later
            boardTypeMultiplier: 1
        };
    }

    componentDidMount(){
        const multiplier = this.getBoardTypeMultiplier(this.props.game.parameters.regionCount);
        this.setState({
            boardTypeMultiplier: multiplier
        });
    }

    private getBoardTypeMultiplier(regionCount : number) : number {
        //Through trial an error, I found that this formula keeps boards
        //of varying regionCount about the same absolute size
        return Math.pow(Math.E, (-0.2 * regionCount)) * 2;
    }

    private getZoomMultiplierFromZoomLevel(zoomLevel : number) : number {
        switch (zoomLevel) {
            case -3: return 0.25;
            case -2: return 0.5;
            case -1: return 0.75;
            case 0: return 1;
            case 1: return 1.25;
            case 2: return 1.5;
            case 3: return 2;
            case 4: return 3;
            case 5: return 4;
            default: throw "Unsupported zoom level: " + zoomLevel;
        }
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
        const p = this.props;
        const bv = p.boardView;

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
                        board={bv}
                        theme={p.theme}
                        selectCell={(cell) => p.selectCell(cell)}
                        magnification={
                            this.state.zoomMultiplier *
                            this.state.boardTypeMultiplier *
                            this.state.windowSizeMultiplier
                        }
                    />
                </Scrollbars>
            </div>
        );
    }

    private renderZoomControl() {
        return (

            <div>
                <LabeledInput
                    label="Zoom"
                    type={InputTypes.Range}
                    onChange={e => this.onZoomSliderChanged(e)}
                    value={this.state.zoomLevel.toString()}
                    min={-3}
                    max={5}
                />
                {this.getZoomDescription()}
            </div>
        );
    }
}