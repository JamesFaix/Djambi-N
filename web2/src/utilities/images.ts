import { PieceKind } from "../api/model";
import { Dispatch } from "redux";
import * as Actions from '../store/actions';

function getPieceImagePath(kind : PieceKind) : string {
    switch (kind) {
        case PieceKind.Assassin: return "../../resources/daggerEmoji.png";
        case PieceKind.Chief: return "../../resources/crownEmoji.png";
        case PieceKind.Corpse: return "../../resources/skullEmoji.png";
        case PieceKind.Diplomat: return "../../resources/doveEmoji.png";
        case PieceKind.Gravedigger: return "../../resources/pickEmoji.png";
        case PieceKind.Reporter: return "../../resources/newspaperEmoji.png";
        case PieceKind.Thug: return "../../resources/fistEmoji.png";
        default: throw "Invalid piece kind.";
    }
}

function createPieceImage(kind : PieceKind, dispatch : Dispatch) : HTMLImageElement {
    const image = new (window as any).Image() as HTMLImageElement;
    image.src = getPieceImagePath(kind);
    image.onload = () => dispatch(Actions.loadPieceImage(kind, image));
    return image;
}

export function init(dispatch : Dispatch) : void {
    const kinds = [
        PieceKind.Assassin,
        PieceKind.Chief,
        PieceKind.Corpse,
        PieceKind.Diplomat,
        PieceKind.Gravedigger,
        PieceKind.Reporter,
        PieceKind.Thug
    ];
    kinds.forEach(k => createPieceImage(k, dispatch));
}