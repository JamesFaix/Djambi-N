import { Line, Point } from './model';
import * as P from './point';
import { xors } from '../utilities/logic';

export function create(a: Point, b: Point): Line {
  return { a, b };
}

// Point a fraction of the way down a line.
// Generalization of midpoint.
// midpoint(L) = fractionPoint(L, 0.5)
export function fractionPoint(l: Line, fraction: number): Point {
  const fr = Math.max(0, Math.min(1, fraction)); // Limit to between 0 and 1
  const compliment = 1 - fr;

  return {
    x: (l.a.x * compliment) + (l.b.x * fr),
    y: (l.a.y * compliment) + (l.b.y * fr),
  };
}

// Determines if both lines share exactly one vertex, within the given threshold of error
export function isChainedTo(l1: Line, l2: Line, threshold: number): boolean {
  return xors([
    P.isCloseTo(l1.a, l2.a, threshold),
    P.isCloseTo(l1.a, l2.b, threshold),
    P.isCloseTo(l1.b, l2.a, threshold),
    P.isCloseTo(l1.b, l2.b, threshold),
  ]);
}

// Determines if both lines share both vertices, within the given threshold of error
// Ignores orientation of each line
export function isCloseTo(l1: Line, l2: Line, threshold: number): boolean {
  return (P.isCloseTo(l1.a, l2.a, threshold) && P.isCloseTo(l1.b, l2.b, threshold))
    || (P.isCloseTo(l1.a, l2.b, threshold) && P.isCloseTo(l1.b, l2.a, threshold));
}

// TS compiler won't let you use `length` because of `Function.length`
export function len(l: Line): number {
  const dX = l.a.x - l.b.x;
  const dY = l.a.y - l.b.y;
  // eslint-disable-next-line no-restricted-properties
  return Math.abs(Math.sqrt(Math.pow(dX, 2) + Math.pow(dY, 2)));
}

export function midPoint(l: Line): Point {
  return {
    x: (l.a.x + l.b.x) / 2,
    y: (l.a.y + l.b.y) / 2,
  };
}
