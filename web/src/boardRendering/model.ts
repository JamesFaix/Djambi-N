import { Polygon } from "../geometry/model";

export enum CellState {
    Default,
    Selected,
    Selectable
}

export enum CellType {
    White,
    Black,
    Seat
}

export interface CellView {
    id : number,
    type : CellType,
    state : CellState,
    polygons : Polygon[],
}

export interface BoardView {
    regionCount : number,
    cellSize : number,
    cells : CellView[]
}