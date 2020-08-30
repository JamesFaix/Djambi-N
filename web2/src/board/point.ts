import * as MathJs from 'mathjs';
import { Point } from './model';

export function add(a: Point, b: Point): Point {
  return {
    x: a.x + b.x,
    y: a.y + b.y,
  };
}

export function addScalar(p: Point, n: number): Point {
  return {
    x: p.x + n,
    y: p.y + n,
  };
}

export function create(x: number, y: number): Point {
  return { x, y };
}

export function distance(a: Point, b: Point): number {
  const dx = a.x - b.x;
  const dy = a.y - b.y;
  return Math.sqrt(dx * dx + dy * dy);
}

export function divide(a: Point, b: Point): Point {
  return {
    x: a.x / b.x,
    y: a.y / b.y,
  };
}

export function divideSafe(a: Point, b: Point): Point {
  return {
    x: b.x === 0 ? 0 : a.x / b.x,
    y: b.y === 0 ? 0 : a.y / b.y,
  };
}

export function isCloseTo(a: Point, b: Point, threshold: number): boolean {
  return distance(a, b) < threshold;
}

export function max(a: Point, b: Point): Point {
  return {
    x: Math.max(a.x, b.x),
    y: Math.max(a.y, b.y),
  };
}

export function multiply(a: Point, b: Point): Point {
  return {
    x: a.x * b.x,
    y: a.y * b.y,
  };
}

export function multiplyScalar(p: Point, n: number): Point {
  return {
    x: p.x * n,
    y: p.y * n,
  };
}

export function subtract(a: Point, b: Point): Point {
  return {
    x: a.x - b.x,
    y: a.y - b.y,
  };
}

export function subtractScalar(p: Point, n: number): Point {
  return {
    x: p.x - n,
    y: p.y - n,
  };
}

export function toString(p: Point): string {
  return `(${p.x}, ${p.y})`;
}

// TODO: Add unit tests
export function transform(p: Point, matrix: MathJs.Matrix): Point {
  const pointVector = MathJs.matrix([p.x, p.y, 1]);
  const resultMatrix = MathJs.multiply(matrix, pointVector);
  // eslint-disable-next-line
  const resultArray = (resultMatrix as any)._data as number[]; // Breaking MathJs's encapsulation here for efficiency
  return {
    x: resultArray[0] / resultArray[2],
    y: resultArray[1] / resultArray[2],
  };
}

export function zero(): Point {
  return {
    x: 0,
    y: 0,
  };
}
