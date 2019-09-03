import { CellView, CellType, CellState, CellHighlight } from "../viewModel/board/model";
import Color from "../viewModel/board/color";

export default class ThemeService {
    public static applyToCss(theme : Theme) : void {
        const docStyle = document.documentElement.style;

        docStyle.setProperty("--background-color", theme.colors.background);
        docStyle.setProperty("--text-color", theme.colors.text);
        docStyle.setProperty("--border-color", theme.colors.border);
        docStyle.setProperty("--hover-text-color", theme.colors.hoverText);
        docStyle.setProperty("--hover-background-color", theme.colors.hoverBackground);

        docStyle.setProperty("--player-color-0", theme.colors.player0);
        docStyle.setProperty("--player-color-1", theme.colors.player1);
        docStyle.setProperty("--player-color-2", theme.colors.player2);
        docStyle.setProperty("--player-color-3", theme.colors.player3);
        docStyle.setProperty("--player-color-4", theme.colors.player4);
        docStyle.setProperty("--player-color-5", theme.colors.player5);
        docStyle.setProperty("--player-color-6", theme.colors.player6);
        docStyle.setProperty("--player-color-7", theme.colors.player7);
    }

    public static getPlayerColor(theme : Theme, playerColorId : number) : string {
        switch(playerColorId) {
            case 0: return theme.colors.player0;
            case 1: return theme.colors.player1;
            case 2: return theme.colors.player2;
            case 3: return theme.colors.player3;
            case 4: return theme.colors.player4;
            case 5: return theme.colors.player5;
            case 6: return theme.colors.player6;
            case 7: return theme.colors.player7;
            default: throw "Unsupported player color id: " + playerColorId;
        }
    }

    public static getCellColor(theme : Theme, cell : CellView) : string {
        const baseColor = ThemeService.getCellBaseColor(theme, cell.type);
        const highlight = ThemeService.getCellHighlight(theme, cell.state);
        if (!highlight){
            return baseColor;
        } else {
            return Color.fromHex(baseColor)
                .lighten(highlight.intensity)
                .multiply(Color.fromHex(highlight.color))
                .toHex();
        }
    }

    private static getCellBaseColor(theme : Theme, type : CellType) {
        const c = theme.colors.cells;
        switch(type){
            case CellType.Center: return c.center;
            case CellType.Even: return c.even;
            case CellType.Odd: return c.odd;
            default: throw "Unsupported celltype: " + type;
        }
    }

    private static getCellHighlight(theme : Theme, state : CellState) : CellHighlight {
        const c = theme.colors.cells;
        switch (state)
        {
            case CellState.Default:
                return null;

            case CellState.Selected:
                return {
                    color: c.selectedColor,
                    intensity: c.selectedIntensity
                };

            case CellState.Selectable:
                return {
                    color: c.selectableColor,
                    intensity: c.selectableIntensity
                };

            default:
                throw "Invalid cell state: " + state;
        }
    }
}