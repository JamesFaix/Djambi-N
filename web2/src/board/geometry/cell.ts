import * as MathJs from 'mathjs';
import { Point, Rectangle, CellView } from './model';
import * as P from './polygon';

export function boundingBox(c: CellView): Rectangle {
  return P.boundingBox(c.polygon);
}

export function centroid(c: CellView): Point {
  return P.centroid(c.polygon);
}

export function transform(c: CellView, matrix: MathJs.Matrix): CellView {
  return {
    ...c,
    polygon: P.transform(c.polygon, matrix),
  };
}
