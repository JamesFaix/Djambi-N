const Logic = {
    xor(a : boolean, b : boolean) : boolean {
        return a ? !b : b;
    },

    xors(xs : boolean[]) : boolean {
        return xs.reduce(this.Xor, false);
    }
};

export default Logic;