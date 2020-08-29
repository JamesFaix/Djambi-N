import * as P from './point';
import * as L from './line';

describe('line', () => {
  describe('fractionPoint', () => {
    it('Returns the point 1/2-way down the line', () => {
      const l = {
        a: P.zero(),
        b: { x: 0, y: 8 },
      };
      const expected = { x: 0, y: 4 };
      const actual = L.fractionPoint(l, 0.5);
      expect(actual).toStrictEqual(expected);
    });

    it('Returns the point 1/4-way down the line', () => {
      const l = {
        a: P.zero(),
        b: { x: 0, y: 8 },
      };
      const expected = { x: 0, y: 2 };
      const actual = L.fractionPoint(l, 0.25);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('isChainedTo', () => {
    it('Returns false if ict(l1.a, l2.a) and ict(l1.b, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.05, y: 0.05 },
        b: { x: 0.95, y: 0.95 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if ict(l1.a, l2.b) and ict(l1.b, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.95, y: 0.95 },
        b: { x: 0.05, y: 0.05 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns true if only ict(l1.a, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.05, y: 0.05 },
        b: { x: 5, y: 5 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns true if only ict(l1.b, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 5, y: 5 },
        b: { x: 0.95, y: 0.95 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns true if only ict(l1.a, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 5, y: 5 },
        b: { x: 0.05, y: 0.05 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns true if only ict(l1.b, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.95, y: 0.95 },
        b: { x: 5, y: 5 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns false if no matches', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 2, y: 2 },
        b: { x: 3, y: 3 },
      };
      const actual = L.isChainedTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });
  });

  describe('isCloseTo', () => {
    it('Returns true if ict(l1.a, l2.a) and ict(l1.b, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.05, y: 0.05 },
        b: { x: 0.95, y: 0.95 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns true if ict(l1.a, l2.b) and ict(l1.b, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.95, y: 0.95 },
        b: { x: 0.05, y: 0.05 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(true);
    });

    it('Returns false if only ict(l1.a, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.05, y: 0.05 },
        b: { x: 5, y: 5 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if only ict(l1.b, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 5, y: 5 },
        b: { x: 0.95, y: 0.95 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if only ict(l1.a, l2.b)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 5, y: 5 },
        b: { x: 0.05, y: 0.05 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if only ict(l1.b, l2.a)', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 0.95, y: 0.95 },
        b: { x: 5, y: 5 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });

    it('Returns false if no matches', () => {
      const threshold = 0.1;
      const l1 = {
        a: { x: 0, y: 0 },
        b: { x: 1, y: 1 },
      };
      const l2 = {
        a: { x: 2, y: 2 },
        b: { x: 3, y: 3 },
      };
      const actual = L.isCloseTo(l1, l2, threshold);
      expect(actual).toBe(false);
    });
  });

  describe('len', () => {
    it('Returns the right length', () => {
      const l = {
        a: P.zero(),
        b: { x: 1, y: 1 },
      };
      const expected = Math.sqrt(2);
      const actual = L.len(l);
      expect(actual).toBe(expected);
    });
  });

  describe('midPoint', () => {
    it('Returns the point 1/2-way down the line', () => {
      const l = {
        a: P.zero(),
        b: { x: 0, y: 8 },
      };
      const expected = { x: 0, y: 4 };
      const actual = L.midPoint(l);
      expect(actual).toStrictEqual(expected);
    });
  });
});
