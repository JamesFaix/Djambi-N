import { expect } from 'chai';
import Geometry from '../../../src/boardRendering/geometry';
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
        expect(actual).to.eql(expected);
    });

    it('Returns the point 1/4-way down the line', () => {
        let l = {
            a : P.zero(),
            b : { x: 0, y : 8 }
        };
        let expected = { x: 0, y: 2 };
        let actual = L.fractionPoint(l, 0.25);
        expect(actual).to.eql(expected);
    });
});

describe('Line.isChainedTo', () => {
    it('Returns false if ict(l1.a, l2.a) and ict(l1.b, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.05, y: 0.05 },
            b: { x: 0.95, y: 0.95 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns false if ict(l1.a, l2.b) and ict(l1.b, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.95, y: 0.95 },
            b: { x: 0.05, y: 0.05 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns true if only ict(l1.a, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.05, y: 0.05 },
            b: { x: 5, y: 5 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns true if only ict(l1.b, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 5, y: 5 },
            b: { x: 0.95, y: 0.95 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns true if only ict(l1.a, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 5, y: 5 },
            b: { x: 0.05, y: 0.05 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns true if only ict(l1.b, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.95, y: 0.95 },
            b: { x: 5, y: 5 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns false if no matches', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 2, y: 2 },
            b: { x: 3, y: 3 }
        };
        let actual = L.isChainedTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });
});

describe('Line.isCloseTo', () => {
    it('Returns true if ict(l1.a, l2.a) and ict(l1.b, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.05, y: 0.05 },
            b: { x: 0.95, y: 0.95 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns true if ict(l1.a, l2.b) and ict(l1.b, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.95, y: 0.95 },
            b: { x: 0.05, y: 0.05 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(true);
    });

    it('Returns false if only ict(l1.a, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.05, y: 0.05 },
            b: { x: 5, y: 5 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns false if only ict(l1.b, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 5, y: 5 },
            b: { x: 0.95, y: 0.95 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns false if only ict(l1.a, l2.b)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 5, y: 5 },
            b: { x: 0.05, y: 0.05 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns false if only ict(l1.b, l2.a)', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 0.95, y: 0.95 },
            b: { x: 5, y: 5 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(false);
    });

    it('Returns false if no matches', () => {
        let threshold = 0.1;
        let l1 = {
            a: { x: 0, y: 0 },
            b: { x: 1, y: 1 }
        };
        let l2 = {
            a: { x: 2, y: 2 },
            b: { x: 3, y: 3 }
        };
        let actual = L.isCloseTo(l1, l2, threshold);
        expect(actual).to.equal(false);
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
        expect(actual).to.eql(expected);
    });
});