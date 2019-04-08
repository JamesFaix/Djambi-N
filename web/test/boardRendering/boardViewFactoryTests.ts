import BoardViewFactory from '../../src/boardRendering/boardViewFactory';
import { expect } from 'chai';
import { CellType } from '../../src/boardRendering/model';
import Geometry from '../../src/boardRendering/geometry';
const Location = Geometry.Location;
const Polygon = Geometry.Polygon;
const Point = Geometry.Point;

describe('BoardViewFactory.getRegionPolygon', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.getRowOrColumnBorderDistanceFromRegionEdge', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.getRowBorders', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.getCellPolygon', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.getCellView', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.createEmptyBoardView', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.mergePartialCellViews', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.mergeCellViews', () => {
    //TODO: Add tests
});

describe('BoardViewFactory.mergePolygons', () => {
    it('Should merge adjacent squares', () => {
        let ps = [
            Polygon.create([
                Point.create(0, 0),
                Point.create(0, 1),
                Point.create(1, 1),
                Point.create(1, 0)
            ]),
            Polygon.create([
                Point.create(0, 1),
                Point.create(1, 1),
                Point.create(1, 2),
                Point.create(0, 2)
            ])
        ];
        let expected = Polygon.create([
            Point.create(0, 2),
            Point.create(0, 1),
            Point.create(0, 0),
            Point.create(1, 0),
            Point.create(1, 1),
            Point.create(1, 2)
        ]);
        let actual = BoardViewFactory.mergePolygons(ps);
        expect(actual).to.eql(expected);
    });
});

describe('BoardViewFactory.getCellType', () => {
    it('Should return Odd if (row+column) is odd', () => {
        let loc = Location.create(NaN, 1, 2);
        let actual = BoardViewFactory.getCellType(loc);
        expect(actual).to.equal(CellType.Odd);
    });

    it('Should return Event if (row+column) is even', () => {
        let loc = Location.create(NaN, 1, 3);
        let actual = BoardViewFactory.getCellType(loc);
        expect(actual).to.equal(CellType.Even);
    });

    it('Should return Center if row and column are 0', () => {
        let loc = Location.create(NaN, 0, 0);
        let actual = BoardViewFactory.getCellType(loc);
        expect(actual).to.equal(CellType.Center);
    });
});