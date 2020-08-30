import { Polygon, Point } from './model';
import * as P from './point';

const sideLength = 1;

function isDivisibleBy(divisor: number, dividend: number): boolean {
  return dividend % divisor === 0;
}

function isEven(n: number): boolean {
  return isDivisibleBy(2, n);
}

function internalAngle(numberOfSides: number): number {
  // Divide a circle into n slices
  return (2 * Math.PI) / numberOfSides;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function externalAngle(numberOfSides: number): number {
  /*
      Internal angle forms a "slice" with the internal angle and 2 other equal angles.
      Those two outer angles of the slice are each 1/2 of the external angle of the polygon.
      The sum of the angles of a triangle is PI radians.
      So, PI minus the internal angle is the sum of the other 2 angles of the "slice".
  */
  return Math.PI - internalAngle(numberOfSides);
}

export function sideToRadiusRatio(numberOfSides: number): number {
  /*
      Radius connects center and a vertex.

      Start with a "slice" triangle, as described in the externalAngle derivation.
      One side of the "slice" is an edge of the polygon, the other two are radii.
      The "slice" can be divided in half by adding a line from the center of the polygon
      to the mid-point of the edge. (This line is an apothem.)

      This "half-slice" is a right triangle. Its sides are one radius, one apothem,
      and a half-edge. The angles that are PI/2, internalAngle/2 and externalAngle/2 radians.

      Thus:
          1) sin(internalAngle/2) = (edge/2)/radius
          2) cos(internalAngle/2) = apothem/radius
          3) tan(internalAngle/2) = (edge/2)/apothem
          4) sin(externalAngle/2) = apothem/radius
          5) cos(externalAngle/2) = (edge/2)/radius
          6) tan(externalAngle/2) = apothem/(edge/2)

      From 1),
          sin(internalAngle/2) = (edge/2)/radius
          radius * sin(internalAngle/2) = edge/2
          radius = (edge/2)/sin(internalAngle/2)
  */
  return (sideLength / 2) / Math.sin(internalAngle(numberOfSides) / 2);
}

export function sideToApothemRatio(numberOfSides: number): number {
  /*
      Apothem connects center and the mid-point of an edge.

      From 3) in the derivation of sideToRadiusRatio,
          tan(internalAngle/2) = (edge/2)/apothem
          apothem * tan(internalAngle/2) = edge/2
          apothem = (edge/2)/tan(internalAngle/2)
  */
  return (sideLength / 2) / Math.tan(internalAngle(numberOfSides) / 2);
}

// eslint-disable-next-line no-shadow
export function create(numberOfSides: number, sideLength: number): Polygon {
  /*
      Creates a regular n-gon with the given side length.
      It's center will be (0,0).
      The bottom side will be parallel to, and below, the x-axis.
  */
  const radius = sideToRadiusRatio(numberOfSides) * sideLength;
  const ia = internalAngle(numberOfSides);

  const vertices: Point[] = [];
  let angle = ia / 2;
  for (let i = 1; i <= numberOfSides; i += 1) {
    const p = {
      x: radius * Math.sin(angle),
      y: radius * Math.cos(angle),
    };
    vertices.push(p);
    angle += ia;
  }
  return { vertices };
}

export function sideToHeightRatio(numberOfSides: number): number {
  /*
      This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.
      For even n, there will be two edges parallel to the x-axis, for odd n just one.
      Thus, for even n the height is (2 * apothem), and for odd n it is (apothem + radius).
  */
  const a = sideToApothemRatio(numberOfSides);

  return isEven(numberOfSides)
    ? a * 2
    : a + sideToRadiusRatio(numberOfSides);
}

export function sideToWidthRatio(numberOfSides: number): number {
  /*
      This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.
      Width is a lot harder to come up with a general formula for than the previous functions.
  */
  if (numberOfSides === 3 || numberOfSides === 4) {
    // The width of an equilateral triangle or a square is obviously the length of its side.
    return 1;
  } if (isDivisibleBy(4, numberOfSides)) {
    // For squares, octogons, 12-gons, 16-gons, etc, the width will always equal the height.
    return sideToApothemRatio(numberOfSides) * 2;
  } if (isEven(numberOfSides)) {
    // For hexagons, 10-gons, 14-gons, etc, the width will always be 2 * radius.
    return sideToRadiusRatio(numberOfSides) * 2;
  }
  // For all other odd n-gons, I do not know of a simple formula.
  // The next best heuristic is to generate an n-gon, and then check
  // the min and max X values of its vertices.
  const polygon = create(numberOfSides, 1);
  const ys = polygon.vertices.map((p) => p.y);
  const maxY = Math.max(...ys);
  const minY = Math.min(...ys);
  return Math.abs(maxY - minY);
}

export function sideToSizeRatios(numberOfSides: number): Point {
  return {
    x: sideToWidthRatio(numberOfSides),
    y: sideToHeightRatio(numberOfSides),
  };
}

export function sideToCentroidOffsetFromTopLeftRatios(numberOfSides: number): Point {
  /*
      This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.

      Since the polygon's bottom is parallel to the x-axis, it is always symmetric
      across the y-axis. Thus the distance from the left is (width/2).

      Since the polygon's bottom is parallel to the x-axis, the distance from the top to the
      centroid follows two patterns.
      For even n, the top will also be parallel and there will be an apothem connecting the
      center to the top, and for odd n, the top will be a vertex and there will be a radius
      connecting the center to the top.
  */
  return {
    x: sideToWidthRatio(numberOfSides) / 2,
    y: isEven(numberOfSides)
      ? sideToApothemRatio(numberOfSides)
      : sideToRadiusRatio(numberOfSides),
  };
}

export function sideToCentroidOffsetFromCenterRatios(numberOfSides: number): Point {
  const centroidOffset = sideToCentroidOffsetFromTopLeftRatios(numberOfSides);
  const centerOffset: Point = {
    x: sideToWidthRatio(numberOfSides) / 2,
    y: sideToHeightRatio(numberOfSides) / 2,
  };
  return P.subtract(centroidOffset, centerOffset);
}
