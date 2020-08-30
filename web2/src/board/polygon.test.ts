import * as P from './polygon';

describe('polygon', () => {
  describe('boundingBox', () => {
    it('Should return "self" for square with no rotation', () => {
      const p = {
        vertices: [
          { x: 0, y: 0 },
          { x: 0, y: 1 },
          { x: 1, y: 1 },
          { x: 1, y: 0 },
        ],
      };
      const expected = {
        left: 0,
        right: 1,
        top: 0,
        bottom: 1,
      };
      const actual = P.boundingBox(p);
      expect(actual).toStrictEqual(expected);
    });

    it('Should return rectangle w/ min and max values of polygon', () => {
      const p = {
        vertices: [
          { x: 0, y: 0 },
          { x: 0, y: 1 },
          { x: 1, y: 0 },
        ],
      };
      const expected = {
        left: 0,
        right: 1,
        top: 0,
        bottom: 1,
      };
      const actual = P.boundingBox(p);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('centroid', () => {
    it('Returns center of square', () => {
      const p = {
        vertices: [
          { x: 0, y: 0 },
          { x: 0, y: 1 },
          { x: 1, y: 1 },
          { x: 1, y: 0 },
        ],
      };
      const expected = { x: 0.5, y: 0.5 };
      const actual = P.centroid(p);
      expect(actual).toStrictEqual(expected);
    });

    it('Returns correct value for triangle', () => {
      const p = {
        vertices: [
          { x: 0, y: 0 },
          { x: 0, y: 1 },
          { x: 1, y: 0 },
        ],
      };
      const expected = { x: (1 + 0 + 0) / 3, y: (0 + 1 + 0) / 3 };
      const actual = P.centroid(p);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('edges', () => {
    it('Returns correct lines', () => {
      const p = {
        vertices: [
          { x: 0, y: 0 },
          { x: 0, y: 1 },
          { x: 1, y: 0 },
        ],
      };
      const expected = [
        {
          a: p.vertices[0],
          b: p.vertices[1],
        },
        {
          a: p.vertices[1],
          b: p.vertices[2],
        },
        {
          a: p.vertices[2],
          b: p.vertices[0],
        },
      ];
      const actual = P.edges(p);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('height', () => {
    it('Returns difference of highest and lowest vertex y components', () => {
      const p = {
        vertices: [
          { x: 10, y: 0 },
          { x: 5, y: -3 },
          { x: 1, y: 1 },
        ],
      };
      const expected = 4;
      const actual = P.height(p);
      expect(actual).toBe(expected);
    });
  });

  // TODO: Add tests for Polygon.transform

  describe('width', () => {
    it('Returns difference of highest and lowest vertex x components', () => {
      const p = {
        vertices: [
          { x: 10, y: 0 },
          { x: 5, y: -3 },
          { x: 1, y: 1 },
        ],
      };
      const expected = 9;
      const actual = P.width(p);
      expect(actual).toBe(expected);
    });
  });
});
