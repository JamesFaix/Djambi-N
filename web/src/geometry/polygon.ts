import Point from "./point";
import Line from "./line";

export default class Polygon {
    constructor (readonly vertices: Array<Point>) {
        if (!(vertices.length > 2)) {
            throw "Polygon must have more than 2 vertices.";
        }
    }

    edges() {
        const verticesOffset = this.vertices.slice(1);
        verticesOffset.push(this.vertices[0]);

        const result = [];
        for (var i = 0; i < this.vertices.length; i++) {
            result.push(new Line(this.vertices[i], verticesOffset[i]));
        }
        return result;
    }

    contains(point : Point) {
        function getSideOfLine(pt: Point, line: Line) {
            return ((pt.x - line.a.x) * (line.b.y - line.a.y))
                - ((pt.y - line.a.y) * (line.b.x - line.a.x));
        }

        let pos = 0;
        let neg = 0;

        const ls = this.edges();
        for (var i = 0; i < ls.length; i++) {
            const side = getSideOfLine(point, ls[i]);
            if (side < 0) { neg++; }
            else if (side > 0) { pos++; }
        }

        return pos === ls.length || neg === ls.length;
    }

    centroid() {
        let sumX = 0;
        let sumY = 0;

        const count = this.vertices.length;

        for (var i = 0; i < count; i++) {
            const c = this.vertices[i];
            sumX += c.x;
            sumY += c.y;
        }

        return new Point(sumX / count, sumY / count);
    }

    translate(offset : Point) : Polygon {
        return new Polygon(this.vertices.map(v => v.translate(offset)));
    }

    transform(matrix : Array<Array<number>>) : Polygon {
        return new Polygon(this.vertices.map(v => v.transform(matrix)));
    }
}