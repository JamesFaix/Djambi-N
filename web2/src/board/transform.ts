import * as MathJs from 'mathjs';
import { Point } from './model';

// https://www.mathworks.com/help/images/matrix-representation-of-geometric-transformations.html

export function identity(): MathJs.Matrix {
  return MathJs.matrix([
    [1, 0, 0],
    [0, 1, 0],
    [0, 0, 1],
  ]);
}

export function compose(transforms: MathJs.Matrix[]): MathJs.Matrix {
  switch (transforms.length) {
    case 0:
      return identity();

    case 1:
      return transforms[0];

    default: {
      let t = transforms[0];
      for (let i = 1; i < transforms.length; i += 1) {
        t = MathJs.multiply(t, transforms[i]) as MathJs.Matrix;
      }
      return t;
    }
  }
}

export function flipHorizontal(): MathJs.Matrix {
  return MathJs.matrix([
    [-1, 0, 0],
    [0, 1, 0],
    [0, 0, 1],
  ]);
}

export function flipVertical(): MathJs.Matrix {
  return MathJs.matrix([
    [1, 0, 0],
    [0, -1, 0],
    [0, 0, 1],
  ]);
}

export function rotation(degrees: number): MathJs.Matrix {
  const radians = (degrees / 180) * Math.PI;
  const sin = Math.sin(radians);
  const cos = Math.cos(radians);

  return MathJs.matrix([
    [cos, sin, 0],
    [-sin, cos, 0],
    [0, 0, 1],
  ]);
}

export function scale(size: Point): MathJs.Matrix {
  return MathJs.matrix([
    [size.x, 0, 0],
    [0, size.y, 0],
    [0, 0, 1],
  ]);
}

export function translate(size: Point): MathJs.Matrix {
  return MathJs.matrix([
    [1, 0, size.x],
    [0, 1, size.y],
    [0, 0, 1],
  ]);
}
