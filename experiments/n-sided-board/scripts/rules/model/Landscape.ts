import {Location} from "./Location.js";
import {RadialDirections} from "./RadialDirections.js";
import {Directions, DirectionsUtility} from "./Directions.js";
import {Cell} from "./Cell.js";

export class Landscape {
    constructor(
        readonly regionCount : number,
        readonly regionSize : number
    ){
    }

    colocations(location : Location) : Array<Location> {
        const results : Array<Location> = [];

        if (location.region >= this.regionCount
            || location.x >= this.regionSize
            || location.y >= this.regionSize) {
            return results;
        }

        if (location.x === 0 && location.y === 0) {
            for (var i = 0; i < this.regionCount; i++){
                results.push(new Location(0, 0, i));
            }
            return results;
        }

        results.push(location);

        if (location.x === 0) {
            const nextRegion = this.nextRegion(location.region, RadialDirections.CounterClockwise);
            results.push(new Location(location.y, location.x, nextRegion));
        }

        if (location.y === 0) {
            const nextRegion = this.nextRegion(location.region, RadialDirections.Clockwise);
            results.push(new Location(location.y, location.x, nextRegion));            
        }

        return results;
    }
    
    nextRegion(region : number, direction : RadialDirections) : number {
        return direction === RadialDirections.Clockwise
            ? (region + this.regionCount - 1) % this.regionCount
            : (region + 1) % this.regionCount
    }

    nextLocation(location : Location, direction : Directions) : Location {
        const next = location.next(direction);

        //Out of bounds
        if (next.x >= this.regionSize || next.y >= this.regionSize) {
            return undefined;
        }

        if (next.x < 0) {
            //Going counterclockwise across regions (+1 region)
            return new Location(next.y, -next.x, 
                this.nextRegion(next.region, RadialDirections.CounterClockwise));
        }

        if (next.y < 0) {
            //Going clockwise across regions (-1 region)
            return new Location(-next.y, next.x, 
                this.nextRegion(next.region, RadialDirections.Clockwise));
        }

        return next;
    }

    neighbors(cell : Cell) : Array<Cell> {
        if (cell.isCenter()) {
            const results = [];
            for (var i = 0; i < this.regionCount; i++) {
                results.push(new Cell([
                    new Location(1, 0, i),
                    new Location(1, 1, i),
                    new Location(0, 1, i),
                ]));
            }
            return results;
        } else {
            return DirectionsUtility.directions
                .map(d => this.nextLocation(cell.locations[0], d))
                .filter(loc => loc !== undefined)
                .map(loc => new Cell(this.colocations(loc)));
        }
    }

    getPaths(cell : Cell) : Array<Array<Cell>> {
        const results : Array<Array<Cell>> = [];

        if (cell.isCenter()) {
            cell.locations.forEach(loc => {
                [Directions.Up, Directions.UpRight].forEach(d => {
                    let loc1 = loc;
                    let loc2 = this.nextLocation(loc1, d);
                    let path : Array<Cell> = [];
                    for (var i = 0; i < this.regionSize && loc2 !== undefined; i++) {
                        path.push(new Cell(this.colocations(loc2)));
                        loc1 = loc2;
                        loc2 = this.nextLocation(loc1, d);
                    }
                    if (path.length > 0) {
                        results.push(path);
                    }
                });
            });
        } else {
            DirectionsUtility.directions.forEach(d => {
                let loc1 = cell.locations[0];
                let loc2 = this.nextLocation(loc1, d);
                let path : Array<Cell> = [];

                for (var i = 0; i < this.regionSize * 2 && loc2 !== undefined; i++) {
                    path.push(new Cell(this.colocations(loc2)));
                    
                    if (loc2.isCenter()){      
                        let next : Location;

                        if (this.regionCount % 2 == 0){
                            /*
                                (x, y, r) -D-> (0, 0, r) -(D+4)-> (x, y, (r + regionCount/2) % regionCount)
                            */

                            d = DirectionsUtility.rotate(d, 4, RadialDirections.Clockwise);
                            let r = (loc1.region + (this.regionCount/2)) % this.regionCount;
                            next = new Location(loc1.x, loc1.y, r);
                        }
                        else{
                            /*
                                (1, 1, r) -DownLeft-> (0, 0, r) -Up->      (0, 1, (r + Floor(regionCount/2)) % regionCount)
                                (1, 0, r) -Left->     (0, 0, r) -UpRight-> (1, 1, (r + Floor(regionCount/2)) % regionCount)
                                (0, 1, r) -Down->     (0, 0, r) -UpRight-> (1, 1, (r + Ceil(regionCount/2)) % regionCount)
                            */

                            if (d === Directions.DownLeft) {
                                d = Directions.Up;
                                let r = (loc1.region + (Math.floor(this.regionCount / 2))) % this.regionCount;
                                next = new Location(0, 1, r);


                            }else if (d === Directions.Left) {
                                d = Directions.UpRight;
                                let r = (loc1.region + (Math.floor(this.regionCount / 2))) % this.regionCount;
                                next = new Location(1, 1, r);

                            }else if (d === Directions.Down) {
                                d = Directions.UpRight;
                                let r = (loc1.region + (Math.ceil(this.regionCount / 2))) % this.regionCount;
                                next = new Location(1, 1, r);
                            }
                        }

                        //Set to 0,0 in the new region so regionDiff = 0 on next iteration
                        loc1 = new Location(0, 0, next.region);
                        loc2 = next;
                    }
                    else{
                        //Direction rotates 90 degrees when crossing region boundary
                        let regionDiff = loc1.region - loc2.region;
                        
                        const maxRegion = this.regionCount - 1;
                        if ((regionDiff < 0 && regionDiff > -maxRegion) || regionDiff === maxRegion) {
                            d = DirectionsUtility.rotate(d, 2, RadialDirections.CounterClockwise);
                        } else if ((regionDiff > 0 && regionDiff < maxRegion) || regionDiff === -maxRegion) {
                            d = DirectionsUtility.rotate(d, 2, RadialDirections.Clockwise);
                        }

                        loc1 = loc2;
                        loc2 = this.nextLocation(loc1, d);
                    }
                }
                if (path.length > 0) {
                    results.push(path);
                }
            });
        }

        return results;
    }
}