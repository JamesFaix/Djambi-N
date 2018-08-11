import {Location} from "./Location.js";

export class Cell {
    constructor(
        readonly locations : Array<Location>
    ){

    }

    isCenter() {
        return this.locations[0].isCenter();
    }

    is(other : Cell) : boolean {
        return this.locations.find(l1 => 
                other.locations.find(l2 => l1.equals(l2))
                !== undefined) 
            !== undefined;    
    }

    toString() {
        let result = "Cell [" + this.locations[0].toString();

        for (var i = 1; i < this.locations.length; i++){
            result += ", " + this.locations[i].toString();
        }

        result += "]";
        return result;
    }
}