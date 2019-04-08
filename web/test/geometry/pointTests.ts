import { expect } from 'chai';
import Geometry from '../../src/boardRendering/geometry';
const P = Geometry.Point;

describe('Point.add', () => {
    it('Adds points component-wise', () => {
        let p1 = { x: 1, y: 2 };
        let p2 = { x: 4, y: -7 };
        let expected = { x: 5, y: -5 };
        let actual = P.add(p1, p2);
        expect(actual).to.eql(expected);
    });
});

describe('Point.addScalar', () => {
    it('Adds scalar to each component', () => {
        let p = { x: 1, y: 2 };
        let s = 4;
        let expected = { x: 5, y: 6 };
        let actual = P.addScalar(p, s);
        expect(actual).to.eql(expected);
    });
});

describe('Point.divide', () => {
    it('Divides points component-wise', () => {
        let p1 = { x: 1, y: 3 };
        let p2 = { x: 2, y: 4 };
        let expected = { x: 0.5, y: 0.75 };
        let actual = P.divide(p1, p2);
        expect(actual).to.eql(expected);
    });

    it('Treats division by 0 as infinity', () => {
        let p1 = { x: 1, y: 3 };
        let p2 = { x: 0, y: 4 };
        let actual = P.divide(p1, p2);
        expect(isFinite(actual.x)).to.equal(false);
        expect(actual.y).to.equal(0.75);
    });
});

describe('Point.divideSafe', () => {
    it('Divides points component-wise', () => {
        let p1 = { x: 1, y: 3 };
        let p2 = { x: 2, y: 4 };
        let expected = { x: 0.5, y: 0.75 };
        let actual = P.divide(p1, p2);
        expect(actual).to.eql(expected);
    });

    it('Treats division by 0 as 0', () => {
        let p1 = { x: 1, y: 3 };
        let p2 = { x: 0, y: 4 };
        let expected = { x: 0, y: 0.75 };
        let actual = P.divideSafe(p1, p2);
        expect(actual).to.eql(expected);
    });
});

describe('Point.multiply', () => {
    it('Multiplies points component-wise', () => {
        let p1 = { x: 1, y: 2 };
        let p2 = { x: 4, y: -7 };
        let expected = { x: 4, y: -14 };
        let actual = P.multiply(p1, p2);
        expect(actual).to.eql(expected);
    });
});

describe('Point.multiplyScalar', () => {
    it('Multiplies scalar to each component', () => {
        let p = { x: 1, y: 2 };
        let s = 4;
        let expected = { x: 4, y: 8 };
        let actual = P.multiplyScalar(p, s);
        expect(actual).to.eql(expected);
    });
});

describe('Point.subtract', () => {
    it('Subtracts points component-wise', () => {
        let p1 = { x: 1, y: 2 };
        let p2 = { x: 4, y: -7 };
        let expected = { x: -3, y: 9 };
        let actual = P.subtract(p1, p2);
        expect(actual).to.eql(expected);
    });
});

describe('Point.subtractScalar', () => {
    it('Subtracts scalar to each component', () => {
        let p = { x: 1, y: 2 };
        let s = 4;
        let expected = { x: -3, y: -2 };
        let actual = P.subtractScalar(p, s);
        expect(actual).to.eql(expected);
    });
});

describe ('Point.toString', () => {
    it ('Looks right', () => {
        let p = { x: 1, y: -2 };
        let expected = "(1, -2)";
        let actual = P.toString(p);
        expect(actual).to.equal(expected);
    })
});

//TODO: Add tests for Point.transform

describe('Point.zero', () => {
    it('Has 0 for both components', () => {
        let actual = P.zero();
        let expected = { x: 0, y: 0 };
        expect(actual).to.eql(expected);
    });
});