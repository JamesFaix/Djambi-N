import { Polygon } from "../geometry/model";

export enum CellState {
    Default,
    Selected,
    Selectable
}

export enum CellType {
    Even,
    Odd,
    Center
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