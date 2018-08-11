import { Directions } from "./Directions.js";

export class Location {
    constructor(
        readonly x: number,
        readonly y: number,
        readonly region: number
    ){

    }

    equals(other : Location) : boolean {
        return other.x === this.x
            && other.y === this.y
            && other.region === this.region;
    }

    toString() : string {
        return "Region" + this.region + " (" + this.x + ", " + this.y + ")";
    }

    next(direction : Directions) : Location {
        switch (direction) {
            case Directions.Up:
                return new Location(this.x, this.y + 1, this.region);
            case Directions.UpRight:
                return new Location(this.x + 1, this.y + 1, this.region);
            case Directions.Right:
                return new Location(this.x + 1, this.y, this.region);
            case Directions.DownRight:
                return new Location(this.x + 1, this.y - 1, this.region);
            case Directions.Down:
                return new Location(this.x, this.y - 1, this.region);
            case Directions.DownLeft:
                return new Location(this.x - 1, this.y - 1, this.region);
            case Directions.Left:
                return new Location(this.x - 1, this.y, this.region);
            case Directions.UpLeft:
                return new Location(this.x - 1, this.y + 1, this.region);
        }
    }

    isCenter() : boolean {
        return this.x === 0 && this.y === 0;
    }
}