export default class Util {
    static toStringSafe(x : any) {
        if (x === null || x === undefined) {
            return "";
        }

        return x.toString();
    }
}