import { expect } from 'chai';
import Logic from '../src/logic';
import 'mocha';

describe('Logic.Xor', () => {
    it('Returns true if only A is true', () => {
        let actual = Logic.xor(true, false);
        expect(actual).to.equal(true);
    });

    it('Returns true if only B is true', () => {
        let actual = Logic.xor(false, true);
        expect(actual).to.equal(true);
    });

    it('Returns false if A and B are true', () => {
        let actual = Logic.xor(true, true);
        expect(actual).to.equal(false);
    });

    it('Returns false if A and B are false', () => {
        let actual = Logic.xor(false, false);
        expect(actual).to.equal(false);
    });
});

describe('Logic.Xors', () => {
    it('Returns false if zero trues', () => {
        let xs = [false, false, false, false];
        let actual = Logic.xors(xs);
        expect(actual).to.equal(false);
    });

    it('Returns true if one true', () => {
        let xs = [false, true, false, false];
        let actual = Logic.xors(xs);
        expect(actual).to.equal(true);
    });

    it('Returns false if two trues', () => {
        let xs = [false, true, true, false];
        let actual = Logic.xors(xs);
        expect(actual).to.equal(false);
    });

    it('Returns false if empty list', () => {
        let xs : boolean[] = [];
        let actual = Logic.xors(xs);
        expect(actual).to.equal(false);
    });
});