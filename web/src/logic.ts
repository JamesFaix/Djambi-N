export default class Logic {
    public static Xor(a : boolean, b : boolean) : boolean {
        return a ? !b : b;
    }

    public static Xors(xs : boolean[]) : boolean {
        return xs.reduce(this.Xor, false);
    }
}