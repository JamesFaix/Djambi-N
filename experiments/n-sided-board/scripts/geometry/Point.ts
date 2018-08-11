export class Point {
    constructor(
        readonly x : number, 
        readonly y : number) {
    }

    toString() {
        return "(" + this.x + ", " + this.y + ")";
    }

    translate(offset : Point) : Point {
        return new Point(
            this.x + offset.x, 
            this.y + offset.y)
    }

    transform(matrix : Array<Array<number>>) : Point {
        const xt = (this.x * matrix[0][0]) + (this.y * matrix[1][0]) + (matrix[2][0]);
        const yt = (this.x * matrix[0][1]) + (this.y * matrix[1][1]) + (matrix[2][1]);
        const zt = (this.x * matrix[0][2]) + (this.y * matrix[1][2]) + (matrix[2][2]);
        return new Point(xt/zt, yt/zt);
    }
}