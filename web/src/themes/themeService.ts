import { PieceKind, Selection, SelectionKind, Game } from "../api/model";
import * as Sprintf from "sprintf-js";
import Theme from "./theme";
import ThemeFactory from "./themeFactory";
import { CellType, CellState, CellHighlight } from "../boardRendering/model";

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

    public getCellBaseColor(type : CellType) : string {
        switch(type) {
            case CellType.Center: return this.getValue(t => t.cellColorCenter);
            case CellType.Even: return this.getValue(t => t.cellColorEven);
            case CellType.Odd: return this.getValue(t => t.cellColorOdd);
            default: throw "Invalid cell type.";
        }
    }

    public getCellHighlight(state : CellState) : CellHighlight {
        switch (state)
        {
            case CellState.Default:
                return null;

            case CellState.Selected:
                return {
                    color: this.getValue(t => t.cellHighlightSelectedColor),
                    intensity: this.getValue(t => t.cellHighlightSelectedIntensity)
                };

            case CellState.Selectable:
                return {
                    color: this.getValue(t => t.cellHighlightSelectionOptionColor),
                    intensity: this.getValue(t => t.cellHighlightSelectionOptionIntensity)
                };

            default:
                throw "Invalid cell state.";
        }
    }

    public getCenterCellName() : string {
        return this.getValue(t => t.centerCellName);
    }

    public getPieceImage(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.getValue(t => t.pieceImageAssassin);
            case PieceKind.Chief: return this.getValue(t => t.pieceImageChief);
            case PieceKind.Corpse: return this.getValue(t => t.pieceImageCorpse);
            case PieceKind.Diplomat: return this.getValue(t => t.pieceImageDiplomat);
            case PieceKind.Gravedigger: return this.getValue(t => t.pieceImageGravedigger);
            case PieceKind.Reporter: return this.getValue(t => t.pieceImageReporter);
            case PieceKind.Thug: return this.getValue(t => t.pieceImageThug);
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
            case null : return null; //Neutral
            default: throw "Invalid colorId. " + colorId;
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