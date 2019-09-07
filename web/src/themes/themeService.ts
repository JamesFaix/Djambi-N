import { CellView, CellType } from "../viewModel/board/model";
import { PieceKind } from "../api/model";
import { Dispatch } from "redux";
import * as StoreDisplay from '../store/display';
import ThemeFactory from "./themeFactory";
import LocalStorageService from "../utilities/localStorageService";
import { Theme } from './model';
import Images, { PieceImageInfo } from "../utilities/images";

export default class ThemeService {

    //#region Theme initialization

    public static changeTheme(themeName : string, dispatch : Dispatch) : void {
        LocalStorageService.themeName = themeName;
        const theme = ThemeFactory.getThemes().get(themeName);
        dispatch(StoreDisplay.Actions.changeTheme(theme));
        ThemeService.applyToCss(theme);
        ThemeService.loadThemeImages(theme, dispatch);
    }

    private static applyToCss(theme : Theme) : void {
        const s = document.documentElement.style;
        const c  = theme.colors;

        s.setProperty("--background-color", c.background);
        s.setProperty("--text-color", c.text);
        s.setProperty("--header-text-color", c.headerText);
        s.setProperty("--border-color", c.border);
        s.setProperty("--hover-text-color", c.hoverText);
        s.setProperty("--hover-background-color", c.hoverBackground);
        s.setProperty("--alt-row-background-color", c.altRowBackground);
        s.setProperty("--alt-row-text-color", c.altRowText);

        s.setProperty("--player-color-0", c.players.p0);
        s.setProperty("--player-color-1", c.players.p1);
        s.setProperty("--player-color-2", c.players.p2);
        s.setProperty("--player-color-3", c.players.p3);
        s.setProperty("--player-color-4", c.players.p4);
        s.setProperty("--player-color-5", c.players.p5);
        s.setProperty("--player-color-6", c.players.p6);
        s.setProperty("--player-color-7", c.players.p7);

        s.setProperty("--font-family", theme.fonts.normalFamily);
        s.setProperty("--header-font-family", theme.fonts.headerFamily);
    }

    private static loadThemeImages(theme : Theme, dispatch : Dispatch) : void {
        const kinds = [
            PieceKind.Assassin,
            PieceKind.Chief,
            PieceKind.Diplomat,
            PieceKind.Gravedigger,
            PieceKind.Reporter,
            PieceKind.Thug
        ];
        const maxPlayerColorId = 7;

        kinds.forEach(k => {
            for (let i=0; i<=maxPlayerColorId; i++) {
                ThemeService.createPieceImage(theme, k, i, dispatch);
            }
            ThemeService.createPieceImage(theme, k, null, dispatch); //Neutral sprite for abandoned pieces
        });

        ThemeService.createPieceImage(theme, PieceKind.Corpse, null, dispatch); //Corpses are only ever neutral
    }

    private static createPieceImage(theme : Theme, kind : PieceKind, colorId : number, dispatch : Dispatch) : HTMLImageElement {
        let image = new (window as any).Image() as HTMLImageElement;
        image.src = ThemeService.getPieceImagePath(theme, kind);
        image.onload = () => {

            const dummyColor = theme.colors.players.placeholder;
            const playerColor = ThemeService.getPlayerColor(theme, colorId);
            image = Images.replaceColor(image, dummyColor, playerColor);

            const info : PieceImageInfo = {
                kind: kind,
                playerColorId: colorId,
                image: image
            };

            dispatch(StoreDisplay.Actions.pieceImageLoaded(info));
        };
        return image;
    }

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

    //#endregion

    public static loadSavedTheme(dispatch : Dispatch) : void {
        return ThemeService.changeTheme(ThemeService.getSavedThemeName(), dispatch);
    }

    private static getSavedThemeName() : string {
        let name = LocalStorageService.themeName;
        return name ? name : ThemeFactory.default.name;
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