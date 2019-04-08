export class List {
    public static flatMap<T1, T2>(xs : T1[], projection : (x:T1) => T2[]) : T2[] {
        return this.flatten(xs.map(projection));
    }

    public static flatten<T>(xs : T[][]) : T[] {
        return xs.reduce((a, b) => a.concat(b), []);
    }

    public static groupMatches<T>(
        elements : T[],
        areMatch : (a:T, b:T) => boolean) : T[][] {

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
        areMatch : (a:T, b:T) => boolean,
        merge : (a:T, b:T) => T) : T[] {

        const results : T[] = [];

        elements.forEach(el => {
            //Merge with existing match if one found
            for (let i = 0; i < results.length; i++) {
                let r = results[i];
                if (areMatch(el, r)) {
                    results[i] = merge(el, r);
                    return;
                }
            }
            //Otherwise add to list
            results.push(el);
        });

        return results;
    }
}