import { expect } from 'chai';
import { describe, it } from 'mocha';
import { List } from '../../src/utilities/collections';

describe('List.contains', () => {
    it('Returns true if array contains value', () => {
        let xs = [1,2,3];
        let actual = List.contains(xs, 1);
        expect(actual).to.equal(true);
    });

    it('Returns false if array does not contain value', () => {
        let xs = [1,2,3];
        let actual = List.contains(xs, 4);
        expect(actual).to.equal(false);
    });

    it('Returns false if array just contains structurally equal object', () => {
        let xs = [{x: 1}, {x: 2}];
        let actual = List.contains(xs, {x: 1});
        expect(actual).to.equal(false);
    });

    it('Returns false if array is empty', () => {
        let actual = List.contains([], 1);
        expect(actual).to.equal(false);
    });
});

describe('List.exists', () => {
    it('Returns true if array contains value satisfying predicate', () => {
        let xs = [1,2,3];
        let actual = List.exists(xs, x => x > 2);
        expect(actual).to.equal(true);
    });

    it('Returns false if array does not contain value satifying predicate', () => {
        let xs = [1,2,3];
        let actual = List.exists(xs, x => x > 3);
        expect(actual).to.equal(false);
    });

    it('Returns false if array is empty', () => {
        let actual = List.exists([], x => true);
        expect(actual).to.equal(false);
    });
});

describe('List.flatMap', () => {
    let projection = (n : number) => [n, n*n];

    it('Returns flattened list', () => {
        let xs = [1,2,3];
        let expected = [1,1,2,4,3,9];
        let actual = List.flatMap(xs, projection);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.flatMap([], projection);
        expect(actual).to.eql([]);
    });
});

describe('List.flatten', () => {
    it('Returns all elements of input arrays in order', () => {
        let xs = [
            [1,2,3],
            [4,5,6]
        ];
        let expected = [1,2,3,4,5,6];
        let actual = List.flatten(xs);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.flatten([]);
        expect(actual).to.eql([]);
    });
});

describe('List.forAll', () => {
    it('Returns true if all elements satisfy predicate', () => {
        let xs = [1,2,3];
        let actual = List.forAll(xs, x => x < 4);
        expect(actual).to.equal(true);
    });

    it('Returns false if any element does not satisfy predicate', () => {
        let xs = [1,2,3];
        let actual = List.forAll(xs, x => x < 3);
        expect(actual).to.equal(false);
    });

    it('Returns true if array is empty', () => {
        let actual = List.forAll([], x => true);
        expect(actual).to.equal(true);
    });
});

describe('List.groupBy', () => {
    let keySelector = (x : string) => x[0];

    it('Groups elements', () => {
        let xs = [
            "apple",
            "artichoke",
            "banana",
            "orange",
            "avocado",
            "oatmeal"
        ];
        let expected = [
            ["a", ["apple", "artichoke", "avocado"]],
            ["b", ["banana"]],
            ["o", ["orange", "oatmeal"]]
        ];
        let actual = List.groupBy(xs, keySelector);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.groupBy([], keySelector);
        expect(actual).to.eql([]);
    });
});

describe('List.groupMatches', () => {
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
        let actual = List.groupMatches(xs, areMatch);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.groupMatches([], areMatch);
        expect(actual).to.eql([]);
    });
});

describe('List.mergeMatches', () => {
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
        let actual = List.mergeMatches(xs, areMatch, merge);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.mergeMatches([], areMatch, merge);
        expect(actual).to.eql([]);
    });
});