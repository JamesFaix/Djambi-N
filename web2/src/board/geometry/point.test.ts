import * as P from './point';

describe('point', () => {
  describe('add', () => {
    it('Adds points component-wise', () => {
      const p1 = { x: 1, y: 2 };
      const p2 = { x: 4, y: -7 };
      const expected = { x: 5, y: -5 };
      const actual = P.add(p1, p2);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('addScalar', () => {
    it('Adds scalar to each component', () => {
      const p = { x: 1, y: 2 };
      const s = 4;
      const expected = { x: 5, y: 6 };
      const actual = P.addScalar(p, s);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('distance', () => {
    it('Returns correct value', () => {
      const p1 = { x: 1, y: 1 };
      const p2 = { x: 2, y: 2 };
      const expected = Math.sqrt(2);
      const actual = P.distance(p1, p2);
      expect(actual).toBe(expected);
    });

    it('Is always positive, regardless of direction', () => {
      const p1 = { x: 2, y: 2 };
      const p2 = { x: 1, y: 1 };
      const expected = Math.sqrt(2);
      const actual = P.distance(p1, p2);
      expect(actual).toBe(expected);
    });
  });

  describe('divide', () => {
    it('Divides points component-wise', () => {
      const p1 = { x: 1, y: 3 };
      const p2 = { x: 2, y: 4 };
      const expected = { x: 0.5, y: 0.75 };
      const actual = P.divide(p1, p2);
      expect(actual).toStrictEqual(expected);
    });

    it('Treats division by 0 as infinity', () => {
      const p1 = { x: 1, y: 3 };
      const p2 = { x: 0, y: 4 };
      const actual = P.divide(p1, p2);
      // eslint-disable-next-line no-restricted-globals
      expect(isFinite(actual.x)).toBe(false);
      expect(actual.y).toBe(0.75);
    });
  });

  describe('divideSafe', () => {
    it('Divides points component-wise', () => {
      const p1 = { x: 1, y: 3 };
      const p2 = { x: 2, y: 4 };
      const expected = { x: 0.5, y: 0.75 };
      const actual = P.divide(p1, p2);
      expect(actual).toStrictEqual(expected);
    });

    it('Treats division by 0 as 0', () => {
      const p1 = { x: 1, y: 3 };
      const p2 = { x: 0, y: 4 };
      const expected = { x: 0, y: 0.75 };
      const actual = P.divideSafe(p1, p2);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('isCloseTo', () => {
    it('Returns true if distance is less than 0.1', () => {
      const p1 = { x: 0, y: 0 };
      const p2 = { x: 0, y: 0.05 };
      const threshold = 0.1;
      const actual = P.isCloseTo(p1, p2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns false if distance is equal to 0.1', () => {
      const p1 = { x: 0, y: 0 };
      const p2 = { x: 0, y: 0.1 };
      const threshold = 0.1;
      const actual = P.isCloseTo(p1, p2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if distance is greater than 0.1', () => {
      const p1 = { x: 0, y: 0 };
      const p2 = { x: 0, y: 0.2 };
      const threshold = 0.1;
      const actual = P.isCloseTo(p1, p2, threshold);
      expect(actual).toBe(false);
    });
  });

  describe('multiply', () => {
    it('Multiplies points component-wise', () => {
      const p1 = { x: 1, y: 2 };
      const p2 = { x: 4, y: -7 };
      const expected = { x: 4, y: -14 };
      const actual = P.multiply(p1, p2);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('multiplyScalar', () => {
    it('Multiplies scalar to each component', () => {
      const p = { x: 1, y: 2 };
      const s = 4;
      const expected = { x: 4, y: 8 };
      const actual = P.multiplyScalar(p, s);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('subtract', () => {
    it('Subtracts points component-wise', () => {
      const p1 = { x: 1, y: 2 };
      const p2 = { x: 4, y: -7 };
      const expected = { x: -3, y: 9 };
      const actual = P.subtract(p1, p2);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('subtractScalar', () => {
    it('Subtracts scalar to each component', () => {
      const p = { x: 1, y: 2 };
      const s = 4;
      const expected = { x: -3, y: -2 };
      const actual = P.subtractScalar(p, s);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('toString', () => {
    it('Looks right', () => {
      const p = { x: 1, y: -2 };
      const expected = '(1, -2)';
      const actual = P.toString(p);
      expect(actual).toBe(expected);
    });
  });

  // TODO: Add tests for Point.transform

  describe('zero', () => {
    it('Has 0 for both components', () => {
      const actual = P.zero();
      const expected = { x: 0, y: 0 };
      expect(actual).toStrictEqual(expected);
    });
  });
});
