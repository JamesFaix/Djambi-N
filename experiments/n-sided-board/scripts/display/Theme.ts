import { GameState, PieceType, Piece, SelectionType, Selection} from "../apiClient/PlayModel.js";
import { VisualBoard } from "./VisualBoard.js";

export interface ITheme {
    getPieceEmoji(piece : Piece) : string;

    getPlayerColor(colorId : number) : string;

    getRequiredSelectionPrompt(requiredSelectionType : SelectionType) : string;

    getPieceTypeName(pieceType : PieceType) : string;

    getCenterName() : string;

    getSelectionDescription(selection: Selection, gameState: GameState, board : VisualBoard) : string;
}

class DefaultTheme {
    constructor() {}

    getPieceEmoji(piece : Piece) : string {
        switch (piece.type) {
            case PieceType.Chief : return "&#x1F451";
            case PieceType.Assassin : return "&#x1F5E1";
            case PieceType.Diplomat : return "&#x1F54A";
            case PieceType.Reporter : return "&#x1F4F0";
            case PieceType.Gravedigger : return "&#x26CF";
            case PieceType.Thug : return "&#x270A";
            case PieceType.Corpse : return "&#x1F480";
            default: throw "Invalid piece type '" +  piece.type + "'";
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

    getRequiredSelectionPrompt(requiredSelectionType : SelectionType) : string {
        switch (requiredSelectionType){
            case null: return "(Click Done or Reset)";
            case SelectionType.Subject: return "Select a piece to move";
            case SelectionType.Move: return "Select a cell to move to";
            case SelectionType.Target: return "Select a piece to target";
            case SelectionType.Drop: return "Select a cell to drop the target piece in";
            case SelectionType.Vacate: return "Select a cell to vacate to";
            default: throw "Invalid selection type";
        }
    }

    getPieceTypeName(pieceType : PieceType) : string {
        switch (pieceType) {
            case PieceType.Assassin: return "Assassin";
            case PieceType.Chief: return "Chief";
            case PieceType.Corpse: return "Corpse";
            case PieceType.Diplomat: return "Diplomat";
            case PieceType.Gravedigger: return "Gravedigger";
            case PieceType.Reporter: return "Reporter";
            case PieceType.Thug: return "Thug";
            default: throw "Invalid piece type";
        }
    }

    getCenterName() : string {
        return "The Seat";
    }

    getSelectionDescription(selection: Selection, gameState: GameState, board: VisualBoard) : string {
        const piece = selection.pieceId === null
            ? null
            : gameState.pieces.find(p => p.id === selection.pieceId);

        const cell = board.cellById(selection.cellId);

        //TODO: Add support for printing cell coordinates, not IDs

        switch (selection.type) {
            case SelectionType.Subject:
                return "Move " + this.getPieceTypeName(piece.type);

            case SelectionType.Move:
                return piece === null  
                    ? " to cell " + cell.id
                    : " to cell " + cell.id + " and target " + this.getPieceTypeName(piece.type);

            case SelectionType.Target:
                return " and target " + this.getPieceTypeName(piece.type) + " at cell " + cell.id;

            case SelectionType.Drop:
                return ", then drop target piece at cell " + cell.id;

            case SelectionType.Vacate:
                return ", finally vacate " + this.getCenterName() + " to cell " + cell.id;

            default:
                throw "Invalid selection type";
        }            
    }
}

class HotDogTownTheme {
    private readonly baseTheme = new DefaultTheme();
    constructor() {}

    getPieceEmoji(piece : Piece) : string {
        switch (piece.type) {
            case PieceType.Chief : return "&#x1F96B";
            case PieceType.Assassin : return "&#x1F374";
            case PieceType.Diplomat : return "&#x1F917";
            case PieceType.Reporter : return "&#x1F4A8";
            case PieceType.Gravedigger : return "&#x1F924";
            case PieceType.Thug : return "&#x1F35F";
            case PieceType.Corpse : return "&#x1F32D";
            default: throw "Invalid piece type '" +  piece.type + "'";
        }
    }

    getPlayerColor(colorId : number) : string {
        return this.baseTheme.getPlayerColor(colorId);
    }   

    getRequiredSelectionPrompt(requiredSelectionType : SelectionType) : string {
        return this.baseTheme.getRequiredSelectionPrompt(requiredSelectionType);
    }

    getPieceTypeName(pieceType : PieceType) : string {
        switch (pieceType) {
            case PieceType.Assassin: return "Fork";
            case PieceType.Chief: return "Sauce";
            case PieceType.Corpse: return "Hotdog";
            case PieceType.Diplomat: return "Hugger";
            case PieceType.Gravedigger: return "Eater";
            case PieceType.Reporter: return "Fart";
            case PieceType.Thug: return "Fries";
            default: throw "Invalid piece type";
        }
    }

    getCenterName() : string {
        return "The Booth";
    }

    getSelectionDescription(selection: Selection, gameState: GameState, board: VisualBoard) : string {
        const piece = selection.pieceId === null
            ? null
            : gameState.pieces.find(p => p.id === selection.pieceId);

        const cell = board.cellById(selection.cellId);

        //TODO: Add support for printing cell coordinates, not IDs

        switch (selection.type) {
            case SelectionType.Subject:
                return "Move " + this.getPieceTypeName(piece.type);

            case SelectionType.Move:
                return piece === null  
                    ? " to cell " + cell.id
                    : " to cell " + cell.id + " and target " + this.getPieceTypeName(piece.type);

            case SelectionType.Target:
                return " and target " + this.getPieceTypeName(piece.type) + " at cell " + cell.id;

            case SelectionType.Drop:
                return ", then drop target piece at cell " + cell.id;

            case SelectionType.Vacate:
                return ", finally vacate " + this.getCenterName() + " to cell " + cell.id;

            default:
                throw "Invalid selection type";
        }            
    }
}

export class ThemeFactory {
    static readonly default : ITheme = new DefaultTheme();
    static readonly hotdogTown : ITheme = new HotDogTownTheme();

    static getThemeNames() : Array<string> {
        return ["default", "hotdogTown"];
    }

    static getTheme(name : string) : ITheme {
        switch (name) {
            case "default": return this.default;
            case "hotdogTown": return this.hotdogTown;
            default: throw "Invalid theme";
        }
    }
}