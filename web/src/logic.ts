const Logic = {
    xor(a : boolean, b : boolean) : boolean {
        return a ? !b : b;
    },

    xors(xs : boolean[]) : boolean {
        return xs.reduce(this.xor, false);
    }
};

export default Logic;