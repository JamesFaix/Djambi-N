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
    b1 : number,
    b2 : number,

    /*
       a1 b1
       a2 b2
    */
}