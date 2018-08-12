namespace Djambi.Model

module BoardGeometry =
    
    type Directions =
        | Up = 1
        | UpRight = 2
        | Right = 3
        | DownRight = 4
        | Down = 5
        | DownLeft = 6
        | Left = 7
        | UpLeft = 8

    type RadialDirections =
        | Clockwise = 1
        | CounterClockwise = 2

    type Location = 
        {
            region : int
            x : int
            y : int
        }

    type Cell =
        {
            id : int
            locations : Location list
        }
      
    type BoardMetadata =
        {
            regionCount : int
            regionSize : int
        }

    type Board = 
        {
            regionCount : int
            regionSize : int
            cells : Cell list
        }

open BoardGeometry
        
module BoardGeometryExtensions =

    open BoardGeometry
    open System.Collections.Generic
    open System.Linq
    open Utilities

    type Directions with 
        member this.rotate(amount : int, radialDirection : RadialDirections) : Directions =
            let value = LanguagePrimitives.EnumToValue this
            let newValue = if radialDirection = RadialDirections.Clockwise
                           then (value - amount) % 8
                           else (value + amount) % 8
            enum<Directions> newValue

    type Location with
        member this.next(direction : Directions) : Location =
            match direction with
                | Directions.Up ->        { this with                 y = this.y + 1 }
                | Directions.UpRight ->   { this with x = this.x + 1; y = this.y + 1 }
                | Directions.Right ->     { this with x = this.x + 1 }
                | Directions.DownRight -> { this with x = this.x + 1; y = this.y - 1 }
                | Directions.Down ->      { this with                 y = this.y - 1 }
                | Directions.DownLeft ->  { this with x = this.x - 1; y = this.y - 1 }
                | Directions.Left ->      { this with x = this.x - 1 }
                | Directions.UpLeft ->    { this with x = this.x - 1; y = this.y + 1 }
                | _ -> failwith (direction.ToString() + " is not a valid " + typeof<Directions>.Name)

        member this.isCenter() : bool =
            this.x = 0 && this.y = 0

        member this.isBorder() : bool =
            this.x = 0 || this.y = 0

    type Cell with 
        member this.isCenter() : bool =
            this.locations 
            |> Seq.exists (fun l -> l.isCenter())

        member this.isColocated(other : Cell) : bool =
            this.locations 
            |> Seq.exists (fun l1 -> other.locations 
                                    |> Seq.exists(fun l2 -> l1 = l2))
                                    
    type BoardMetadata with
        member this.maxRegion() = this.regionCount - 1

        member this.contains(location : Location) : bool =
            location.region >= 0 && location.region < this.regionCount &&
            location.x >= 0 && location.x < this.regionSize &&
            location.y >= 0 && location.y < this.regionSize

        member this.nextRegion(region : int, direction : RadialDirections) : int =
            if direction = RadialDirections.Clockwise
            then (region + this.maxRegion()) % this.regionCount
            else (region + 1) % this.regionCount
            
        member this.colocations(location : Location) : Location list =
            //Empty list if out of bounds
            if this.contains(location) |> not
            then List.empty

            //Center for each region if center            
            else if location.isCenter()
            then [0..(this.maxRegion())] 
                 |> List.map (fun i -> { region = i; x = 0; y = 0 })            

            //Include compliment if on region border
            else if location.isBorder()
            then 
                let dir = if location.x = 0 
                          then RadialDirections.CounterClockwise 
                          else RadialDirections.Clockwise 
                let compliment = 
                    {
                        region = this.nextRegion(location.region, dir)
                        x = location.y
                        y = location.x
                    }
                [location; compliment]

            //Just self otherwise
            else [location]

        member this.cells() : Cell list =
            let cellsInner(landscape : BoardMetadata) =
                let locations = 
                    [0..landscape.maxRegion()]
                    |> Seq.collect (fun r -> 
                        [0..(landscape.regionSize-1)]
                        |> Seq.collect (fun x -> 
                            [0..(landscape.regionSize-1)]
                            |> Seq.map (fun y -> { region = r; x = x; y = y })))
                    |> Enumerable.ToList
            
                let mutable cellId = 1
                let cells = new List<Cell>()

                while locations.Count > 0 do
                    let colocations = landscape.colocations(locations.[0])
                    cells.Add({id = cellId; locations = colocations})
                    cellId <- cellId + 1
                    for cl in colocations do
                        locations.Remove(cl) |> ignore
            
                cells |> Seq.toList

            memoize cellsInner this

        member this.cellAt(location : Location) : Cell =
            let cellAtInner(landscape : BoardMetadata)(loc : Location) =
                landscape.cells()
                |> Seq.find (fun c -> c.locations |> Seq.contains loc)

            let memoized = memoize cellAtInner this
            memoized location

        member this.nextLocation(from : Location, direction : Directions) : Location option =
            let next = from.next(direction)
            //If out of bounds, undefined
            if next.x >= this.regionSize || next.y >= this.regionSize 
            then None
            //If past Y axis, go counterclockwise to next region
            else if next.x < 0
            then Some {
                    region = this.nextRegion(next.region, RadialDirections.CounterClockwise)
                    x = next.y
                    y = -next.x
                 }
            //If past X axis, go clockwise to next region
            else if next.y < 0
            then Some {
                    region = this.nextRegion(next.region, RadialDirections.Clockwise)
                    x = -next.y
                    y = next.x
                 }
            //Otherwise, just next in that region
            else Some next

        member this.neighbors(cell : Cell) : Cell list =
            if cell.isCenter() 
            then [0..(this.maxRegion())] 
                 |> Seq.collect (fun r -> 
                    [
                        { region = r; x = 0; y = 1 } 
                        { region = r; x = 1; y = 1 } 
                        { region = r; x = 1; y = 0 } 
                    ])
            else Utilities.GetValues<Directions>()
                 |> Seq.map (fun d -> this.nextLocation(cell.locations.Head, d))
                 |> Seq.filter (fun o -> o.IsSome)
                 |> Seq.map (fun o -> o.Value)
                 |> Seq.collect this.colocations
            |> Seq.map this.cellAt
            |> Seq.distinct
            |> Seq.toList

        member this.paths(location : Location) : Cell list list =
            this.paths(this.cellAt(location))

        member this.paths(cell : Cell) : Cell list list =
            if cell.isCenter()
            then cell.locations
                 |> List.collect (fun l -> 
                    [Directions.Up; Directions.UpRight] 
                    |> List.map (fun d -> 
                        let mutable loc1 = l
                        let mutable loc2 = this.nextLocation(loc1, d)
                        let mutable i = 0
                        seq {
                            while (i < this.regionSize * 2 && loc2.IsSome) do
                                i <- i + 1
                                yield this.cellAt(loc1)                           
                                loc1 <- loc2.Value
                                loc2 <- this.nextLocation(loc1, d)
                        }                    
                        |> Seq.toList))
            else Utilities.GetValues<Directions>()
                 |> List.map(fun d -> 
                    let mutable dir = d
                    let mutable loc1 = cell.locations.Head
                    let mutable loc2 = this.nextLocation(loc1, dir)
                    let mutable i = 0
                    seq {
                        while (i < this.regionSize * 2 && loc2.IsSome) do
                            i <- i + 1
                            yield this.cellAt(loc1)                         
                            if loc2.Value.isCenter() 
                            then
                                //This isn't really mutated, its just assigned to in one of several places
                                let mutable next : Location option = None
                                if this.regionCount % 2 = 0
                                then           
                                    (*
                                        (x, y, r) -D-> (0, 0, r) -(D+4)-> (x, y, (r + regionCount/2) % regionCount)
                                    *)
                                    dir <- dir.rotate(4, RadialDirections.Clockwise)
                                    let r = (loc1.region + (this.regionCount/2)) % this.regionCount
                                    next <- Some { region = r; x = loc1.x; y = loc1.y }
                                else
                                    (*
                                        (1, 1, r) -DownLeft-> (0, 0, r) -Up->      (0, 1, (r + Floor(regionCount/2)) % regionCount)
                                        (1, 0, r) -Left->     (0, 0, r) -UpRight-> (1, 1, (r + Floor(regionCount/2)) % regionCount)
                                        (0, 1, r) -Down->     (0, 0, r) -UpRight-> (1, 1, (r + Ceil(regionCount/2)) % regionCount)
                                    *)
                                    let almostHalf = int (floor ((float this.regionCount) / 2.0))

                                    match dir with
                                        | Directions.DownLeft -> 
                                            dir <- Directions.Up
                                            let r = (loc1.region + almostHalf) % this.regionCount
                                            next <- Some { region = r; x = 0; y = 1 }
                                        | Directions.Left ->
                                            dir <- Directions.UpRight
                                            let r = (loc1.region + almostHalf) % this.regionCount
                                            next <- Some { region = r; x = 1; y = 1 }
                                        | Directions.Down -> 
                                            dir <- Directions.UpRight
                                            let r = (loc1.region + almostHalf + 1) % this.regionCount
                                            next <- Some { region = r; x = 1; y = 1 }
                                        | _ -> ()
                                            
                                //Set to 0,0 in the new region so regionDiff = 0 on next iteration                    
                                loc1 <- { region = next.Value.region; x = 0; y = 0 };
                                loc2 <- next;
                            else
                                //Direction rotates 90 degrees when crossing region boundary
                                let regionDiff = loc1.region - loc2.Value.region
                                let max = this.maxRegion()
                                if (regionDiff < 0 && regionDiff > max) || regionDiff = max
                                then 
                                    dir <- dir.rotate(2, RadialDirections.CounterClockwise)
                                else if (regionDiff > 0 && regionDiff < max) || regionDiff = -max
                                then 
                                    dir <- dir.rotate(2, RadialDirections.Clockwise)
                                else ()

                                loc1 <- loc2.Value
                                loc2 <- this.nextLocation(loc1, dir)
                    }
                    |> Seq.toList
                 )
            |> List.filter (fun path -> (List.length path) > 0)
            
module BoardRepository =

    open BoardGeometry
    open BoardGeometryExtensions

    let getBoardMetadata(regionCount : int) : BoardMetadata =
        let standardRegionSize = 5
        {
            regionCount = regionCount
            regionSize = standardRegionSize
        }

    let getBoard(regionCount : int) : Board =
        let metadata = getBoardMetadata(regionCount)
        {
            regionCount = metadata.regionCount
            regionSize = metadata.regionSize
            cells = metadata.cells()
        }