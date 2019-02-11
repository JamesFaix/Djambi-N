import { Point, Line, Polygon, TransformMatrix } from "./model";

export default class Geometry {

    //---POINT---

    public static pointTranslate(p : Point, offset : Point) : Point {
        return {
            x: p.x + offset.x,
            y: p.y + offset.y
        }
    }

    public static pointTransform(p : Point, matrix : TransformMatrix) : Point {
        const xt = (p.x * matrix.a1) + (p.y * matrix.a2) + (matrix.a3);
        const yt = (p.x * matrix.b1) + (p.y * matrix.b2) + (matrix.b3);
        const zt = (p.x * matrix.c1) + (p.y * matrix.c2) + (matrix.c3);

        return {
            x: xt/zt,
            y: yt/zt
        };
    }

    public static pointToString(p : Point) : string {
        return "(" + p.x + ", " + p.y + ")";
    }

    //---LINE---

    public static lineLength(l : Line) : number {
        const dX = l.a.x - l.b.x;
        const dY = l.a.y - l.b.y;
        return Math.abs(Math.sqrt(Math.pow(dX, 2) + Math.pow(dY, 2)));
    }

    public static lineMidPoint(l : Line) : Point {
        return {
            x: (l.a.x + l.b.x) / 2,
            y: (l.a.y + l.b.y) / 2
        }
    }

    //Point a fraction of the way down a line.
    //Generalization of midpoint.
    //midpoint(L) = fractionPoint(L, 0.5)
    public static lineFractionPoint(l : Line, fraction : number) : Point {
        fraction = Math.max(0, Math.min(1, fraction)); //Limit to between 0 and 1
        const compliment = 1 - fraction;

        return {
            x: (l.a.x * compliment) + (l.b.x * fraction),
            y: (l.a.y * compliment) + (l.b.y * fraction)
        };
    }

    //---POLYGON---

    public static polygonEdges(p : Polygon) : Line[] {
        const verticesOffset = p.vertices.slice(1);
        verticesOffset.push(p.vertices[0]);

        const result = [];
        for (var i = 0; i < p.vertices.length; i++) {
            const line = {
                a: p.vertices[i],
                b: verticesOffset[i]
            };
            result.push(line);
        }
        return result;
    }

    public static polygonContains(p : Polygon, point : Point) : boolean {
        function getSideOfLine(pt: Point, line: Line) {
            return ((pt.x - line.a.x) * (line.b.y - line.a.y))
                - ((pt.y - line.a.y) * (line.b.x - line.a.x));
        }

        let pos = 0;
        let neg = 0;

        const ls = this.polygonEdges(p);
        for (var i = 0; i < ls.length; i++) {
            const side = getSideOfLine(point, ls[i]);
            if (side < 0) { neg++; }
            else if (side > 0) { pos++; }
        }

        return pos === ls.length || neg === ls.length;
    }

    public static polygonCentroid(p : Polygon) : Point {
        let sumX = 0;
        let sumY = 0;

        const count = p.vertices.length;

        for (var i = 0; i < count; i++) {
            const c = p.vertices[i];
            sumX += c.x;
            sumY += c.y;
        }

        return {
            x: sumX / count,
            y: sumY / count
        };
    }

    public static polygonTranslate(p : Polygon, offset : Point) : Polygon {
        return { vertices : p.vertices.map((v : Point) => this.pointTranslate(v, offset)) };
    }

    public static polygonTransform(p : Polygon, matrix : TransformMatrix) : Polygon {
        return { vertices : p.vertices.map((v : Point) => this.pointTransform(v, matrix)) };
    }

    public static regularPolygonRadius(numberOfSides : number, sideLength : number) : number {
        /*
            A regular polygon P with N sides of length L can be divided radially into N isocolese triangles.
            Given one of these triangles T,
                - The base of T is L
                - The side of T is the radius (R) of P
                - The height of T is the apothem (A) of P
                - The "top" angle of T is 360/N degrees
                - The base angles are (180 - (360/N))/2 = 90-(180/N) degrees

            T can be split vertically into two right triangles, so trig functions can be used.
            Given one of these two triangles U,
                - The base of U is L/2
                - The longer side of U is the R, and the shorter is A
                - Angles are 90, 180/N, and 90-(180/N) degrees
                - sin(180/N) = (L/2)/R
                - R = L/(2 * sin(180/N))

            Convert degrees to radians for JS trig functions
        */

        return sideLength / (2 * Math.sin(Math.PI/numberOfSides));
    }

    public static polygonWidth(p : Polygon) : number {
        const xs = p.vertices.map(v => v.x);
        const min = Math.min(...xs);
        const max = Math.max(...xs);
        return Math.abs(max - min);
    }

    public static polygonHeight(p : Polygon) : number {
        const ys = p.vertices.map(v => v.y);
        const min = Math.min(...ys);
        const max = Math.max(...ys);
        return Math.abs(max - min);
    }

    //---TRANSFORMS---

    public static transformIdentity() : TransformMatrix {
        return {
            a1:1, b1:0, c1:0,
            a2:0, b2:1, c2:0,
            a3:0, b3:0, c3:1
        }
    }

    public static transformInverse() : TransformMatrix {
        return {
            a1:0, b1:1, c1:0,
            a2:1, b2:0, c2:0,
            a3:0, b3:0, c3:1
        }
    }

    public static transformRotation(degrees : number) : TransformMatrix {
        const radians = degrees / 180 * Math.PI;
        const sin = Math.sin(radians);
        const cos = Math.cos(radians);

        return {
            a1:cos, b1:-sin, c1:0,
            a2:sin, b2: cos, c2:0,
            a3:0,   b3:0,    c3:1
        }
    }

    public static transformScale(x : number, y : number) : TransformMatrix {
        return {
            a1:x, b1:0, c1:0,
            a2:0, b2:y, c2:0,
            a3:0, b3:0, c3:1
        }
    }

    public static transformCompose(m : TransformMatrix, n : TransformMatrix) : TransformMatrix {
        return {
            a1: (m.a1 * n.a1) + (m.b1 * n.a2) + (m.c1 * n.a3),
            a2: (m.a1 * n.b1) + (m.b1 * n.b2) + (m.c1 * n.b3),
            a3: (m.a1 * n.c1) + (m.b1 * n.c2) + (m.c1 * n.c3),
            b1: (m.a2 * n.a1) + (m.b2 * n.a2) + (m.c2 * n.a3),
            b2: (m.a2 * n.b1) + (m.b2 * n.b2) + (m.c2 * n.b3),
            b3: (m.a2 * n.c1) + (m.b2 * n.c2) + (m.c2 * n.c3),
            c1: (m.a3 * n.a1) + (m.b3 * n.a2) + (m.c3 * n.a3),
            c2: (m.a3 * n.b1) + (m.b3 * n.b2) + (m.c3 * n.b3),
            c3: (m.a3 * n.c1) + (m.b3 * n.c2) + (m.c3 * n.c3)
        };
    }
}