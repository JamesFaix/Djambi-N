import { CellView, CellType } from "../viewModel/board/model";
import { PieceKind } from "../api/model";
import { Dispatch } from "redux";
import * as StoreDisplay from '../store/display';
import { Theme } from './model';

export default class ThemeService {

    public static getPieceImagePath(theme : Theme, kind : PieceKind) : string {
        const i = theme.images.pieces;
        switch (kind) {
            case PieceKind.Assassin: return i.assassin;
            case PieceKind.Chief: return i.chief;
            case PieceKind.Corpse: return i.corpse;
            case PieceKind.Diplomat: return i.diplomat;
            case PieceKind.Gravedigger: return i.gravedigger;
            case PieceKind.Reporter: return i.reporter;
            case PieceKind.Thug: return i.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#region Colors

    public static getPlayerColor(theme : Theme, playerColorId : number) : string {
        const c = theme.colors;
        switch(playerColorId) {
            case 0: return c.player0;
            case 1: return c.player1;
            case 2: return c.player2;
            case 3: return c.player3;
            case 4: return c.player4;
            case 5: return c.player5;
            case 6: return c.player6;
            case 7: return c.player7;
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
            case PieceKind.Assassin: return c.assassin;
            case PieceKind.Chief: return c.chief;
            case PieceKind.Corpse: return c.corpse;
            case PieceKind.Diplomat: return c.diplomat;
            case PieceKind.Gravedigger: return c.gravedigger;
            case PieceKind.Reporter: return c.reporter;
            case PieceKind.Thug: return c.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#endregion
}