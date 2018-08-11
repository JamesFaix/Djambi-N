import { RadialDirections } from "./RadialDirections.js";

export enum Directions {
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}

export class DirectionsUtility {

    static readonly directions : Array<Directions> = [ 
        Directions.Up, 
        Directions.UpRight, 
        Directions.Right, 
        Directions.DownRight, 
        Directions.Down, 
        Directions.DownLeft, 
        Directions.Left, 
        Directions.UpLeft
    ];

    static rotate(
        direction : Directions, 
        amount : number, 
        radialDirection : RadialDirections) : Directions {

        const index = DirectionsUtility.directions.findIndex(d => d === direction);

        const newIndex = radialDirection === RadialDirections.Clockwise
            ? (index - amount) % 8
            : (index + amount) % 8;

        return DirectionsUtility.directions[newIndex];
    }
}