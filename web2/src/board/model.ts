import { PieceKind, LocationDto } from '../api-client';

export type Point = {
  x: number,
  y: number
};

export type Line = {
  a: Point,
  b: Point
};

export type Polygon = {
  vertices: Point[]
};

export enum CellType {
  Even,
  Odd,
  Center
}

export type PieceView = {
  id: number,
  kind: PieceKind,
  colorId: number | null,
  playerName: string | null
};

export type CellView = {
  id: number,
  locations: LocationDto[],
  type: CellType,
  isSelected: boolean,
  isSelectable: boolean,
  piece: PieceView | null,
  polygon: Polygon
};

export type BoardView = {
  regionCount: number,
  cellCountPerSide: number,
  polygon: Polygon,
  cells: CellView[]
};

export type Rectangle = {
  top: number,
  left: number,
  bottom: number,
  right: number
};
