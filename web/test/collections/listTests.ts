import { expect } from 'chai';
import { List } from '../../src/collections';

describe('List.flatMap', () => {
    let projection = (n:number) => [n, n*n];

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

describe('List.groupMatches', () => {
    let areMatch = (a:string, b:string) => a[0] === b[0];

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
    })
});

describe('List.mergeMatches', () => {
    let areMatch = (a:string, b:string) => a[0] === b[0];
    let merge = (a:string, b:string) => a + ", " + b;

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
            "avocado, artichoke, apple",
            "banana",
            "oatmeal, orange"
        ];
        let actual = List.mergeMatches(xs, areMatch, merge);
        expect(actual).to.eql(expected);
    });

    it('Returns empty if input empty', () => {
        let actual = List.mergeMatches([], areMatch, merge);
        expect(actual).to.eql([]);
    })
});