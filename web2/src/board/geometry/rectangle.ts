import * as P from './point';
import { Point } from './model';

export function largestScaleWithinBox(innerSize: Point, outerSize: Point): number {
  const maxScale = P.divide(outerSize, innerSize);
  return Math.min(maxScale.x, maxScale.y);
}
