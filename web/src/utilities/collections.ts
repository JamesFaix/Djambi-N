export class List {
    public static contains<T>(xs : T[], value : T) : boolean {
        return xs.find(x => x === value) !== undefined;
    }

    public static exists<T>(xs : T[], predicate : (value : T) => boolean) : boolean {
        return xs.find(x => predicate(x)) !== undefined;
    }

    public static flatMap<T1, T2>(xs : T1[], projection : (x : T1) => T2[]) : T2[] {
        return this.flatten(xs.map(projection));
    }

    public static flatten<T>(xs : T[][]) : T[] {
        return xs.reduce((a, b) => a.concat(b), []);
    }

    public static forAll<T>(xs : T[], predicate : (value : T) => boolean) : boolean {
        return xs.find(x => !predicate(x)) === undefined;
    }

    public static groupBy<T, TKey>(xs : T[], keySelector : (x : T) => TKey) : [TKey, T[]][] {
        const groups : [TKey, T[]][] = [];

        xs.forEach(x => {
            const key = keySelector(x);
            const group = groups.find(g => g[0] === key);

            if (group === undefined) {
                groups.push([key, [x]]);
            } else {
                group[1].push(x);
            }
        });

        return groups;
    }

    public static groupMatches<T>(
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

    public static mergeMatches<T>(
        elements : T[],
        areMatch : (a : T, b : T) => boolean,
        merge : (a : T, b : T) => T) : T[] {

        return List.groupMatches(elements, areMatch)
            .map(g => g.reduce((x, y) => merge(x,y)));
    }
}

export class MapUtil {
    public static add<TKey, TValue>(xs : Map<TKey, TValue>, key : TKey, value : TValue) : Map<TKey, TValue> {
        const m = new Map<TKey, TValue>(xs);
        m.set(key, value);
        return m;
    }
}