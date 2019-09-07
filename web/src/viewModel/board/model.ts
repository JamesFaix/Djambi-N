import { Location, PieceKind } from '../../api/model';

export interface Point {
    x : number,
    y : number
}

export interface Line {
    a : Point,
    b : Point
}

export interface Polygon {
    vertices : Point[]
}

export enum CellType {
    Even,
    Odd,
    Center
}

export interface PieceView {
    id : number,
    kind : PieceKind,
    colorId : number
}

export interface CellView {
    id : number,
    locations : Location[],
    type : CellType,
    isSelected : boolean,
    isSelectable : boolean,
    piece : PieceView,
    polygon : Polygon
}

export interface BoardView {
    regionCount : number,
    cellCountPerSide : number,
    polygon : Polygon,
    cells : CellView[]
}

export interface Rectangle {
    top : number,
    left : number,
    bottom : number,
    right : number
}