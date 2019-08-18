import { CellType, CellView, CellState, CellHighlight } from "../viewModel/board/model";
import Color from '../viewModel/board/color';

export default class Colors {
    static getColorFromPlayerColorId(colorId : number) : string {
        switch (colorId) {
            case 0: return "blue";
            case 1: return "red";
            case 2: return "green";
            case 3: return "orange";
            case 4: return "brown";
            case 5: return "teal";
            case 6: return "magenta";
            case 7: return "gold";
            default:
                throw "Unsupported colorID " + colorId;
        }
    }

    static getCellViewColor(cell : CellView) : string {
        const baseColor = this.getCellBaseColor(cell.type);
        const highlight = this.getCellHighlight(cell.state);
        if (!highlight){
            return baseColor;
        } else {
            return Color.fromHex(baseColor)
                .lighten(highlight.intensity)
                .multiply(Color.fromHex(highlight.color))
                .toHex();
        }
    }

    static getCellViewBorderColor(cell : CellView) : string {
        return null;
    }

    private static getCellBaseColor(type : CellType) {
        switch(type){
            case CellType.Center: return "#828282"; //Gray
            case CellType.Even: return "#FFFFFF";
            case CellType.Odd: return "#000000";
            default: throw "Unsupported celltype";
        }
    }

    private static getCellHighlight(state : CellState) : CellHighlight {
        switch (state)
        {
            case CellState.Default:
                return null;

            case CellState.Selected:
                return {
                    color: "#6AC921", //Green
                    intensity: 0.75
                };

            case CellState.Selectable:
                return {
                    color: "#E5E500", //Yellow
                    intensity: 0.5
                };

            default:
                throw "Invalid cell state.";
        }
    }

    public static getBoardBorderColor() : string {
        return "black";
    }
}