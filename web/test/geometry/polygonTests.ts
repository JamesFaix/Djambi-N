import { expect } from 'chai';
import Geometry from '../../src/boardRendering/geometry';
const P = Geometry.Polygon;

describe('Polygon.centroid', () => {
    it('Returns center of square', () => {
        let p = {
            vertices: [
                { x: 0, y: 0 },
                { x: 0, y: 1 },
                { x: 1, y: 1 },
                { x: 1, y: 0 }
            ]
        };
        let expected = { x: 0.5, y: 0.5 };
        let actual = P.centroid(p);
        expect(actual).to.eql(expected);
    });

    it('Returns correct value for triangle', () => {
        let p = {
            vertices: [
                { x: 0, y: 0 },
                { x: 0, y: 1 },
                { x: 1, y: 0 }
            ]
        };
        let expected = { x: (1+0+0)/3, y: (0+1+0)/3 };
        let actual = P.centroid(p);
        expect(actual).to.eql(expected);
    });
});

describe('Polygon.edges', () => {
    it('Returns correct lines', () => {
        let p = {
            vertices: [
                { x: 0, y: 0 },
                { x: 0, y: 1 },
                { x: 1, y: 0 }
            ]
        };
        let expected = [
            {
                a: p.vertices[0],
                b: p.vertices[1]
            },
            {
                a: p.vertices[1],
                b: p.vertices[2]
            },
            {
                a: p.vertices[2],
                b: p.vertices[0]
            }
        ];
        let actual = P.edges(p);
        expect(actual).to.eql(expected);
    });
});

describe('Polygon.height', () => {
    it('Returns difference of highest and lowest vertex y components', () => {
        let p = {
            vertices: [
                { x: 10, y: 0 },
                { x: 5, y: -3 },
                { x: 1, y: 1 }
            ]
        };
        let expected = 4;
        let actual = P.height(p);
        expect(actual).to.equal(expected);
    });
});

//TODO: Add tests for Polygon.transform

describe('Polygon.width', () => {
    it('Returns difference of highest and lowest vertex x components', () => {
        let p = {
            vertices: [
                { x: 10, y: 0 },
                { x: 5, y: -3 },
                { x: 1, y: 1 }
            ]
        };
        let expected = 9;
        let actual = P.width(p);
        expect(actual).to.equal(expected);
    });
});