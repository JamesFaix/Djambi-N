export default class Transforms {
    constructor() {

    }

    static identity() {
        return [
            [1, 0, 0],
            [0, 1, 0],
            [0, 0, 1]
        ];
    }

    static inverse() {
        return [
            [0, 1, 0],
            [1, 0, 0],
            [0, 0, 1]
        ];
    }

    static rotation(degrees : number) {
        const radians = degrees / 180 * Math.PI;
        const sin = Math.sin(radians);
        const cos = Math.cos(radians);

        return [
            [cos, -sin, 0],
            [sin, cos, 0],
            [0, 0, 1]
        ];
    }

    static scale(x : number, y : number) {
        return [
            [x, 0, 0],
            [0, y, 0],
            [0, 0, 1]
        ];
    }

    static compose(a : Array<Array<number>>, b : Array<Array<number>>) {
        return [
            [
                (a[0][0] * b[0][0]) + (a[0][1] * b[1][0]) + (a[0][2] * b[2][0]),
                (a[1][0] * b[0][0]) + (a[1][1] * b[1][0]) + (a[1][2] * b[2][0]),
                (a[2][0] * b[0][0]) + (a[2][1] * b[1][0]) + (a[2][2] * b[2][0])
            ],
            [
                (a[0][0] * b[0][1]) + (a[0][1] * b[1][1]) + (a[0][2] * b[2][1]),
                (a[1][0] * b[0][1]) + (a[1][1] * b[1][1]) + (a[1][2] * b[2][1]),
                (a[2][0] * b[0][1]) + (a[2][1] * b[1][1]) + (a[2][2] * b[2][1])
            ],
            [
                (a[0][0] * b[0][2]) + (a[0][1] * b[1][2]) + (a[0][2] * b[2][2]),
                (a[1][0] * b[0][2]) + (a[1][1] * b[1][2]) + (a[1][2] * b[2][2]),
                (a[2][0] * b[0][2]) + (a[2][1] * b[1][2]) + (a[2][2] * b[2][2])
            ]
        ];
    }
}