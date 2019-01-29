import { Selection, Game, Piece, PieceKind, SelectionKind } from "../api/model";

export interface Theme {
    getPieceEmoji(piece : Piece) : string;

    getPlayerColor(colorId : number) : string;

    getRequiredSelectionPrompt(requiredSelectionKind : SelectionKind) : string;

    getPieceTypeName(pieceType : PieceKind) : string;

    getCenterName() : string;

    getSelectionDescription(selection: Selection, game: Game) : string;
}

class DefaultTheme {
    constructor() {}

    getPieceEmoji(piece : Piece) : string {
        switch (piece.kind) {
            case PieceKind.Chief : return "&#x1F451";
            case PieceKind.Assassin : return "&#x1F5E1";
            case PieceKind.Diplomat : return "&#x1F54A";
            case PieceKind.Reporter : return "&#x1F4F0";
            case PieceKind.Gravedigger : return "&#x26CF";
            case PieceKind.Thug : return "&#x270A";
            case PieceKind.Corpse : return "&#x1F480";
            default: throw "Invalid piece kind '" +  piece.kind + "'";
        }
    }

    getPlayerColor(colorId : number) : string {
        switch (colorId){
            case 0: return "#CC2B08"; //Red
            case 1: return "#47CC08"; //Green
            case 2: return "#08A9CC"; //Blue
            case 3: return "#8D08CC"; //Purple
            case 4: return "#CC8708"; //Orange
            case 5: return "#CC0884"; //Pink
            case 6: return "#08CC8B"; //Teal
            case 7: return "#996A0C"; //Brown
            default: throw "Invalid player colorId: " + colorId;
        }
    }

    getRequiredSelectionPrompt(requiredSelectionKind : SelectionKind) : string {
        switch (requiredSelectionKind){
            case null: return "(Click Done or Reset)";
            case SelectionKind.Subject: return "Select a piece to move";
            case SelectionKind.Move: return "Select a cell to move to";
            case SelectionKind.Target: return "Select a piece to target";
            case SelectionKind.Drop: return "Select a cell to drop the target piece in";
            case SelectionKind.Vacate: return "Select a cell to vacate to";
            default: throw "Invalid selection kind";
        }
    }

    getPieceTypeName(pieceKind : PieceKind) : string {
        switch (pieceKind) {
            case PieceKind.Assassin: return "Assassin";
            case PieceKind.Chief: return "Chief";
            case PieceKind.Corpse: return "Corpse";
            case PieceKind.Diplomat: return "Diplomat";
            case PieceKind.Gravedigger: return "Gravedigger";
            case PieceKind.Reporter: return "Reporter";
            case PieceKind.Thug: return "Thug";
            default: throw "Invalid piece kind";
        }
    }

    getCenterName() : string {
        return "The Seat";
    }

    getSelectionDescription(selection: Selection, game: Game) : string {

        const piece = selection.pieceId === null
            ? null
            : game.pieces.find(p => p.id === selection.pieceId);

        //TODO: Add support for printing cell coordinates, not IDs

        switch (selection.kind) {
            case SelectionKind.Subject:
                return "Move " + this.getPieceTypeName(piece.kind);

            case SelectionKind.Move:
                return piece === null
                    ? " to cell " + selection.cellId
                    : " to cell " + selection.cellId + " and target " + this.getPieceTypeName(piece.kind);

            case SelectionKind.Target:
                return " and target " + this.getPieceTypeName(piece.kind) + " at cell " + selection.cellId;

            case SelectionKind.Drop:
                return ", then drop target piece at cell " + selection.cellId;

            case SelectionKind.Vacate:
                return ", finally vacate " + this.getCenterName() + " to cell " + selection.cellId;

            default:
                throw "Invalid selection kind";
        }
    }
}

class HotDogTownTheme {
    private readonly baseTheme = new DefaultTheme();
    constructor() {}

    getPieceEmoji(piece : Piece) : string {
        switch (piece.kind) {
            case PieceKind.Chief : return "&#x1F96B";
            case PieceKind.Assassin : return "&#x1F374";
            case PieceKind.Diplomat : return "&#x1F917";
            case PieceKind.Reporter : return "&#x1F4A8";
            case PieceKind.Gravedigger : return "&#x1F924";
            case PieceKind.Thug : return "&#x1F35F";
            case PieceKind.Corpse : return "&#x1F32D";
            default: throw "Invalid piece kind '" +  piece.kind + "'";
        }
    }

    getPlayerColor(colorId : number) : string {
        return this.baseTheme.getPlayerColor(colorId);
    }

    getRequiredSelectionPrompt(requiredSelectionKind : SelectionKind) : string {
        return this.baseTheme.getRequiredSelectionPrompt(requiredSelectionKind);
    }

    getPieceTypeName(pieceKind : PieceKind) : string {
        switch (pieceKind) {
            case PieceKind.Assassin: return "Fork";
            case PieceKind.Chief: return "Sauce";
            case PieceKind.Corpse: return "Hotdog";
            case PieceKind.Diplomat: return "Hugger";
            case PieceKind.Gravedigger: return "Eater";
            case PieceKind.Reporter: return "Fart";
            case PieceKind.Thug: return "Fries";
            default: throw "Invalid piece kind";
        }
    }

    getCenterName() : string {
        return "The Booth";
    }

    getSelectionDescription(selection: Selection, game: Game) : string {
        const piece = selection.pieceId === null
            ? null
            : game.pieces.find(p => p.id === selection.pieceId);

        //TODO: Add support for printing cell coordinates, not IDs

        switch (selection.kind) {
            case SelectionKind.Subject:
                return "Move " + this.getPieceTypeName(piece.kind);

            case SelectionKind.Move:
                return piece === null
                    ? " to cell " + selection.cellId
                    : " to cell " + selection.cellId + " and target " + this.getPieceTypeName(piece.kind);

            case SelectionKind.Target:
                return " and target " + this.getPieceTypeName(piece.kind) + " at cell " + selection.cellId;

            case SelectionKind.Drop:
                return ", then drop target piece at cell " + selection.cellId;

            case SelectionKind.Vacate:
                return ", finally vacate " + this.getCenterName() + " to cell " + selection.cellId;

            default:
                throw "Invalid selection type";
        }
    }
}

export class ThemeFactory {
    static readonly default : Theme = new DefaultTheme();
    static readonly hotdogTown : Theme = new HotDogTownTheme();

    static getThemeNames() : Array<string> {
        return ["default", "hotdogTown"];
    }

    static getTheme(name : string) : Theme {
        switch (name) {
            case "default": return this.default;
            case "hotdogTown": return this.hotdogTown;
            default: throw "Invalid theme";
        }
    }
}