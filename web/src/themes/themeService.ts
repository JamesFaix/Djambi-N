import { CellView, CellType } from "../viewModel/board/model";
import { PieceKind } from "../api/model";
import { Theme } from './model';
import { NotificationType } from "../store/notifications";

export default class ThemeService {
    public static getPieceImagePath(theme : Theme, kind : PieceKind) : string {
        const i = theme.images.pieces;
        switch (kind) {
            case PieceKind.Hunter: return i.hunter;
            case PieceKind.Conduit: return i.conduit;
            case PieceKind.Corpse: return i.corpse;
            case PieceKind.Diplomat: return i.diplomat;
            case PieceKind.Reaper: return i.reaper;
            case PieceKind.Scientist: return i.scientist;
            case PieceKind.Thug: return i.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#region Colors

    public static getPlayerColor(theme : Theme, playerColorId : number) : string {
        const c = theme.colors.players;
        switch(playerColorId) {
            case 0: return c.p0;
            case 1: return c.p1;
            case 2: return c.p2;
            case 3: return c.p3;
            case 4: return c.p4;
            case 5: return c.p5;
            case 6: return c.p6;
            case 7: return c.p7;
            case null: return c.neutral;
            default: throw "Unsupported player color id: " + playerColorId;
        }
    }

    public static getCellColor(theme : Theme, cell : CellView) : string {
        const c = theme.colors.cells;
        if (cell.isSelected) {
            return c.selectedColor;
        }
        switch(cell.type){
            case CellType.Center: return c.center;
            case CellType.Even: return c.even;
            case CellType.Odd: return c.odd;
            default: throw "Unsupported celltype: " + cell.type;
        }
    }

    public static getCellBorderColor(theme : Theme, type : CellType) {
        const c = theme.colors.cells;
        switch(type){
            case CellType.Center: return c.centerBorder;
            case CellType.Even: return c.evenBorder;
            case CellType.Odd: return c.oddBorder;
            default: throw "Unsupported celltype: " + type;
        }
    }

    public static getCellTextColor(theme : Theme, type : CellType) {
        const c = theme.colors.cells;
        switch(type){
            case CellType.Center: return c.centerText;
            case CellType.Even: return c.evenText;
            case CellType.Odd: return c.oddText;
            default: throw "Unsupported celltype: " + type;
        }
    }

    //#endregion

    //#region Copy

    public static getPieceName(theme : Theme, kind : PieceKind) : string {
        const c = theme.copy.pieces;
        switch (kind) {
            case PieceKind.Hunter: return c.hunter;
            case PieceKind.Conduit: return c.conduit;
            case PieceKind.Corpse: return c.corpse;
            case PieceKind.Diplomat: return c.diplomat;
            case PieceKind.Reaper: return c.reaper;
            case PieceKind.Scientist: return c.scientist;
            case PieceKind.Thug: return c.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#endregion

    public static getNotificationBackground(theme : Theme, type : NotificationType) : string {
        switch (type) {
            case NotificationType.Error:
                return theme.colors.errorBackground;
            case NotificationType.Info:
                return theme.colors.infoBackground;
            default:
                return theme.colors.background;
        }
    }
}
