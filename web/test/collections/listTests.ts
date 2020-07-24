import { expect } from 'chai';
import { describe, it } from 'mocha';
import { exists, groupMatches, mergeMatches, } from '../../src/utilities/collections';

describe('exists', () => {
    it('Returns true if array contains value satisfying predicate', () => {
        let xs = [1,2,3];
        let actual = exists(xs, x => x > 2);
        expect(actual).to.equal(true);
    });

    it('Returns false if array does not contain value satifying predicate', () => {
        let xs = [1,2,3];
        let actual = exists(xs, x => x > 3);
        expect(actual).to.equal(false);
    });

    it('Returns false if array is empty', () => {
        let actual = exists([], x => true);
        expect(actual).to.equal(false);
    });
});

describe('groupMatches', () => {
    let areMatch = (a : string, b : string) => a[0] === b[0];

    it('Groups elements by "areMatch"', () => {
        let xs = [
            "apple",
            "artichoke",
            "banana",
            "orange",
            "avocado",
            "oatmeal"
        ];
        let expected = [
            ["apple", "artichoke", "avocado"],
            ["banana"],
            ["orange", "oatmeal"]
        ];
        let actual = groupMatches(xs, areMatch);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = groupMatches([], areMatch);
        expect(actual).to.eql([]);
    });
});

describe('mergeMatches', () => {
    let areMatch = (a : string, b : string) => a[0] === b[0];
    let merge = (a : string, b : string) => a + ", " + b;

    it('Merges matching elements', () => {
        let xs = [
            "apple",
            "artichoke",
            "banana",
            "orange",
            "avocado",
            "oatmeal"
        ];
        let expected = [
            "apple, artichoke, avocado",
            "banana",
            "orange, oatmeal"
        ];
        let actual = mergeMatches(xs, areMatch, merge);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = mergeMatches([], areMatch, merge);
        expect(actual).to.eql([]);
    });
});