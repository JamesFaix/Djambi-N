export interface Point {
    x : number,
    y : number
}

export interface Line {
    a : Point,
    b : Point
}

export interface Polygon {
    vertices : Point[]
}

export interface TransformMatrix {
    a1 : number,
    a2 : number,
    a3 : number,
    b1 : number,
    b2 : number,
    b3 : number,
    c1 : number,
    c2 : number,
    c3 : number

    /*
       a1 b1 c1
       a2 b2 c2
       a3 b3 c3
    */
}