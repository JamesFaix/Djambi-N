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