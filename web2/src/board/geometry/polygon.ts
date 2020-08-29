import * as MathJs from 'mathjs';
import {
  Polygon, Point, Rectangle, Line,
} from './model';
import * as P from './point';

export function boundingBox(p: Polygon): Rectangle {
  const xs = p.vertices.map((v) => v.x);
  const ys = p.vertices.map((v) => v.y);
  return {
    left: Math.min(...xs),
    right: Math.max(...xs),
    top: Math.min(...ys),
    bottom: Math.max(...ys),
  };
}

export function centroid(p: Polygon): Point {
  let sumX = 0;
  let sumY = 0;

  const count = p.vertices.length;

  for (let i = 0; i < count; i += 1) {
    const c = p.vertices[i];
    sumX += c.x;
    sumY += c.y;
  }

  return {
    x: sumX / count,
    y: sumY / count,
  };
}

export function create(vertices: Point[]): Polygon {
  return { vertices };
}

export function edges(p: Polygon): Line[] {
  const verticesOffset = p.vertices.slice(1);
  verticesOffset.push(p.vertices[0]);

  const result = [];
  for (let i = 0; i < p.vertices.length; i += 1) {
    const line = {
      a: p.vertices[i],
      b: verticesOffset[i],
    };
    result.push(line);
  }
  return result;
}

function size(p: Polygon, projection: (p: Point) => number): number {
  const ys = p.vertices.map((v) => projection(v));
  const min = Math.min(...ys);
  const max = Math.max(...ys);
  return Math.abs(max - min);
}

export function height(p: Polygon): number {
  return size(p, (v) => v.y);
}

// TODO: Add unit tests
export function transform(p: Polygon, matrix: MathJs.Matrix): Polygon {
  return { vertices: p.vertices.map((v: Point) => P.transform(v, matrix)) };
}

export function width(p: Polygon): number {
  return size(p, (v) => v.x);
}
