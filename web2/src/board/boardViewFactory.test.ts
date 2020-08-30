import { getCellType, mergePolygons } from './boardViewFactory';
import * as Location from './location';
import * as Point from './point';
import * as Polygon from './polygon';
import { CellType } from './model';

describe('boardViewFactory', () => {
  describe('mergePolygons', () => {
    it('Should merge adjacent squares', () => {
      const ps = [
        Polygon.create([
          Point.create(0, 0),
          Point.create(0, 1),
          Point.create(1, 1),
          Point.create(1, 0),
        ]),
        Polygon.create([
          Point.create(0, 1),
          Point.create(1, 1),
          Point.create(1, 2),
          Point.create(0, 2),
        ]),
      ];
      const expected = Polygon.create([
        Point.create(0, 2),
        Point.create(0, 1),
        Point.create(0, 0),
        Point.create(1, 0),
        Point.create(1, 1),
        Point.create(1, 2),
      ]);
      const actual = mergePolygons(ps);
      expect(actual).toStrictEqual(expected);
    });
  });

  describe('getCellType', () => {
    it('Should return Odd if (row+column) is odd', () => {
      const loc = Location.create(NaN, 1, 2);
      const actual = getCellType(loc);
      expect(actual).toStrictEqual(CellType.Odd);
    });

    it('Should return Event if (row+column) is even', () => {
      const loc = Location.create(NaN, 1, 3);
      const actual = getCellType(loc);
      expect(actual).toStrictEqual(CellType.Even);
    });

    it('Should return Center if row and column are 0', () => {
      const loc = Location.create(NaN, 0, 0);
      const actual = getCellType(loc);
      expect(actual).toStrictEqual(CellType.Center);
    });
  });
});
