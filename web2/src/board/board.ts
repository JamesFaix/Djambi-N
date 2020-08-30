import * as MathJs from 'mathjs';
import { BoardView, Point } from './model';
import * as P from './polygon';
import * as C from './cell';

export function size(b: BoardView): Point {
  const h = P.height(b.polygon);
  const w = P.width(b.polygon);
  return { x: w, y: h };
}

export function transform(b: BoardView, matrix: MathJs.Matrix): BoardView {
  return {
    ...b,
    polygon: P.transform(b.polygon, matrix),
    cells: b.cells.map((c) => C.transform(c, matrix)),
  };
}
