import { PieceKind, Selection, SelectionKind, Game } from "../api/model";
import * as Sprintf from "sprintf-js";
import Theme from "./theme";
import ThemeFactory from "./themeFactory";
import { CellType, CellState } from "../boardRendering/model";
import Color from "../boardRendering/color";

export default class ThemeService {
    theme : Theme;
    private readonly defaultTheme : Theme;

    constructor(){
        this.defaultTheme = ThemeFactory.getDefaultTheme();
    }

    //Get the custom theme value if it exists, otherwise default value
    private getValue<T>(getProperty : (t : Theme) => T) : T {
        if (this.theme){
            const value = getProperty(this.theme);
            if (value) {
                return value;
            }
        }

        return getProperty(this.defaultTheme);
    }

    public getCellColor(type : CellType, state : CellState) : string {
        let baseColor : string;
        switch(type) {
            case CellType.Black:
                baseColor = this.getValue(t => t.cellColorBlack);
                break;

            case CellType.Seat:
                baseColor = this.getValue(t => t.cellColorCenter);
                break;

            case CellType.White:
                baseColor = this.getValue(t => t.cellColorWhite);
                break;

            default: throw "Invalid cell type.";
        }

        switch (state)
        {
            case CellState.Default:
                return baseColor;

            case CellState.Selected:
                return Color.fromHex(baseColor)
                    .lighten(this.getValue(t => t.cellHighlightSelectedIntensity))
                    .multiply(Color.fromHex(this.getValue(t => t.cellHighlightSelectedColor)))
                    .toHex();

            case CellState.Selectable:
                return Color.fromHex(baseColor)
                    .lighten(this.getValue(t => t.cellHighlightSelectionOptionIntensity))
                    .multiply(Color.fromHex(this.getValue(t => t.cellHighlightSelectionOptionColor)))
                    .toHex();
        }
    }

    public getCenterCellName() : string {
        return this.getValue(t => t.centerCellName);
    }

    public getPieceEmoji(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.getValue(t => t.pieceEmojiAssassin);
            case PieceKind.Chief: return this.getValue(t => t.pieceEmojiChief);
            case PieceKind.Corpse: return this.getValue(t => t.pieceEmojiCorpse);
            case PieceKind.Diplomat: return this.getValue(t => t.pieceEmojiDiplomat);
            case PieceKind.Gravedigger: return this.getValue(t => t.pieceEmojiGravedigger);
            case PieceKind.Reporter: return this.getValue(t => t.pieceEmojiReporter);
            case PieceKind.Thug: return this.getValue(t => t.pieceEmojiThug);
            default: throw "Invalid piece kind.";
        }
    }

    public getPieceName(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.getValue(t => t.pieceNameAssassin);
            case PieceKind.Chief: return this.getValue(t => t.pieceNameChief);
            case PieceKind.Corpse: return this.getValue(t => t.pieceNameCorpse);
            case PieceKind.Diplomat: return this.getValue(t => t.pieceNameDiplomat);
            case PieceKind.Gravedigger: return this.getValue(t => t.pieceNameGravedigger);
            case PieceKind.Reporter: return this.getValue(t => t.pieceNameReporter);
            case PieceKind.Thug: return this.getValue(t => t.pieceNameThug);
            default: throw "Invalid piece kind.";
        }
    }

    public getPlayerColor(colorId : number) : string {
        switch (colorId) {
            case 0: return this.getValue(t => t.playerColor0);
            case 1: return this.getValue(t => t.playerColor1);
            case 2: return this.getValue(t => t.playerColor2);
            case 3: return this.getValue(t => t.playerColor3);
            case 4: return this.getValue(t => t.playerColor4);
            case 5: return this.getValue(t => t.playerColor5);
            case 6: return this.getValue(t => t.playerColor6);
            case 7: return this.getValue(t => t.playerColor7);
            default: throw "Invalid colorId.";
        }
    }

    public getSelectionDescription(selection : Selection, game : Game) : string {
        const piece = selection.pieceId === null
            ? null
            : game.pieces.find(p => p.id === selection.pieceId);

        const pieceName = piece === null
            ? null
            : this.getPieceName(piece.kind);

        //TODO: Add support for using cell coordinates, not IDs
        const cell = selection.cellId;

        let format : string;

        switch (selection.kind) {
            case SelectionKind.Drop:
                format = this.getValue(t => t.selectionDescriptionDrop);
                return Sprintf.sprintf(format, cell);

            case SelectionKind.Move:
                if (piece === null) {
                    format = this.getValue(t => t.selectionDescriptionMove);
                    return Sprintf.sprintf(format, cell);
                } else {
                    format = this.getValue(t => t.selectionDescriptionMoveAndTarget);
                    return Sprintf.sprintf(format, cell, pieceName)
                }

            case SelectionKind.Subject:
                format = this.getValue(t => t.selectionDescriptionSubject);
                return Sprintf.sprintf(format, pieceName);

            case SelectionKind.Target:
                format = this.getValue(t => t.selectionDescriptionTarget);
                return Sprintf.sprintf(format, pieceName, cell);

            case SelectionKind.Vacate:
                format = this.getValue(t => t.selectionDescriptionVacate);
                return Sprintf.sprintf(format, this.getCenterCellName(), cell);

            default: throw "Invalid selection kind.";
        }
    }

    public getSelectionPrompt(kind : SelectionKind) : string {
        switch (kind) {
            case SelectionKind.Drop: return this.getValue(t => t.selectionPromptDrop);
            case SelectionKind.Move: return this.getValue(t => t.selectionPromptMove);
            case SelectionKind.Subject: return this.getValue(t => t.selectionPromptSubject);
            case SelectionKind.Target: return this.getValue(t => t.selectionPromptTarget);
            case SelectionKind.Vacate: return this.getValue(t => t.selectionPromptVacate);
            case null: return this.getValue(t => t.selectionPromptNull);
            default: throw "Invalid selection kind.";
        }
    }
}