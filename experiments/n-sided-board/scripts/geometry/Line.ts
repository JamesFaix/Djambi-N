import {Point} from "./Point.js";

export class Line {
    constructor(
        readonly a: Point, 
        readonly b: Point) {
    }

    midpoint() {
        const x = (this.a.x + this.b.x) / 2;
        const y = (this.a.y + this.b.y) / 2;
        return new Point(x, y);
    }

    fractionPoint(fraction : number) {
        fraction = Math.max(0, Math.min(1, fraction));
        const compliment = 1 - fraction;

        const x = (this.a.x * compliment) + (this.b.x * fraction);
        const y = (this.a.y * compliment) + (this.b.y * fraction);

        return new Point(x, y);
    }
}