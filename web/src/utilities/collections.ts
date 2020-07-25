export function exists<T>(xs : T[], predicate : (value : T) => boolean) : boolean {
    return xs.find(x => predicate(x)) !== undefined;
}

export function groupMatches<T>(
    elements : T[],
    areMatch : (a : T, b : T) => boolean) : T[][] {

    const results : T[][] = [];

    elements.forEach(el => {
        //Add to existing group if found
        for (let i = 0; i < results.length; i++) {
            let r = results[i];
            if (areMatch(el, r[0])) {
                r.push(el);
                return;
            }
        }
        //Otherwise add group
        results.push([el]);
    });

    return results;
}

export function mergeMatches<T>(
    elements : T[],
    areMatch : (a : T, b : T) => boolean,
    merge : (a : T, b : T) => T) : T[] {

    return groupMatches(elements, areMatch)
        .map(g => g.reduce((x, y) => merge(x,y)));
}

export function add<TKey, TValue>(xs : Map<TKey, TValue>, key : TKey, value : TValue) : Map<TKey, TValue> {
    const m = new Map<TKey, TValue>(xs);
    m.set(key, value);
    return m;
}