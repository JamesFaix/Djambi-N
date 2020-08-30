import { exists, groupMatches, mergeMatches } from './collections';

describe('collections', () => {
  describe('exists', () => {
    it('Returns true if array contains value satisfying predicate', () => {
      const xs = [1, 2, 3];
      const actual = exists(xs, (x) => x > 2);
      expect(actual).toBe(true);
    });

    it('Returns false if array does not contain value satifying predicate', () => {
      const xs = [1, 2, 3];
      const actual = exists(xs, (x) => x > 3);
      expect(actual).toBe(false);
    });

    it('Returns false if array is empty', () => {
      const actual = exists([], () => true);
      expect(actual).toBe(false);
    });
  });

  describe('groupMatches', () => {
    const areMatch = (a: string, b: string) => a[0] === b[0];

    it('Groups elements by "areMatch"', () => {
      const xs = [
        'apple',
        'artichoke',
        'banana',
        'orange',
        'avocado',
        'oatmeal',
      ];
      const expected = [
        ['apple', 'artichoke', 'avocado'],
        ['banana'],
        ['orange', 'oatmeal'],
      ];
      const actual = groupMatches(xs, areMatch);
      expect(actual).toStrictEqual(expected);
    });

    it('Returns empty if input empty', () => {
      const actual = groupMatches([], areMatch);
      expect(actual).toStrictEqual([]);
    });
  });

  describe('mergeMatches', () => {
    const areMatch = (a: string, b: string) => a[0] === b[0];
    const merge = (a: string, b: string) => `${a}, ${b}`;

    it('Merges matching elements', () => {
      const xs = [
        'apple',
        'artichoke',
        'banana',
        'orange',
        'avocado',
        'oatmeal',
      ];
      const expected = [
        'apple, artichoke, avocado',
        'banana',
        'orange, oatmeal',
      ];
      const actual = mergeMatches(xs, areMatch, merge);
      expect(actual).toStrictEqual(expected);
    });

    it('Returns empty if input empty', () => {
      const actual = mergeMatches([], areMatch, merge);
      expect(actual).toStrictEqual([]);
    });
  });
});
