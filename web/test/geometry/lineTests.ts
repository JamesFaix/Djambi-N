import { expect } from 'chai';
import Geometry from '../../src/boardRendering/geometry';
const P = Geometry.Point;
const L = Geometry.Line;

describe('Line.fractionPoint', () => {
    it('Returns the point 1/2-way down the line', () => {
        let l = {
            a : P.zero(),
            b : { x: 0, y : 8 }
        };
        let expected = { x: 0, y: 4 };
        let actual = L.fractionPoint(l, 0.5);
        expect(actual.x).to.equal(expected.x);
        expect(actual.y).to.equal(expected.y);
    });

    it('Returns the point 1/4-way down the line', () => {
        let l = {
            a : P.zero(),
            b : { x: 0, y : 8 }
        };
        let expected = { x: 0, y: 2 };
        let actual = L.fractionPoint(l, 0.25);
        expect(actual.x).to.equal(expected.x);
        expect(actual.y).to.equal(expected.y);
    });
});

describe('Line.len', () => {
    it('Returns the right length', () => {
        let l = {
            a : P.zero(),
            b : { x: 1, y : 1 }
        };
        let expected = Math.sqrt(2);
        let actual = L.len(l);
        expect(actual).to.equal(expected);
    });
});

describe('Line.midPoint', () => {
    it('Returns the point 1/2-way down the line', () => {
        let l = {
            a : P.zero(),
            b : { x: 0, y : 8 }
        };
        let expected = { x: 0, y: 4 };
        let actual = L.midPoint(l);
        expect(actual.x).to.equal(expected.x);
        expect(actual.y).to.equal(expected.y);
    });
});