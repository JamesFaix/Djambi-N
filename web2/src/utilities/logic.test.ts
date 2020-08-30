import { xor, xors } from './logic';

describe('logic', () => {
  describe('xor', () => {
    it('Returns true if only A is true', () => {
      const actual = xor(true, false);
      expect(actual).toBe(true);
    });

    it('Returns true if only B is true', () => {
      const actual = xor(false, true);
      expect(actual).toBe(true);
    });

    it('Returns false if A and B are true', () => {
      const actual = xor(true, true);
      expect(actual).toBe(false);
    });

    it('Returns false if A and B are false', () => {
      const actual = xor(false, false);
      expect(actual).toBe(false);
    });
  });

  describe('xors', () => {
    it('Returns false if zero trues', () => {
      const xs = [false, false, false, false];
      const actual = xors(xs);
      expect(actual).toBe(false);
    });

    it('Returns true if one true', () => {
      const xs = [false, true, false, false];
      const actual = xors(xs);
      expect(actual).toBe(true);
    });

    it('Returns false if two trues', () => {
      const xs = [false, true, true, false];
      const actual = xors(xs);
      expect(actual).toBe(false);
    });

    it('Returns false if empty list', () => {
      const xs: boolean[] = [];
      const actual = xors(xs);
      expect(actual).toBe(false);
    });
  });
});
