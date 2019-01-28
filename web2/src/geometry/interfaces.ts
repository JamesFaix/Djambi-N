import Point from "./point";

export interface Transformable<T> {
    transform(matrix : Array<Array<number>>) : T,
    translate(offset : Point) : T
}