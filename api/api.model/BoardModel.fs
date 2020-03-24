[<AutoOpen>]
module Apex.Api.Model.BoardModel

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
        /// <summary> Number of regions on the board. </summary>
        regionCount : int
        /// <summary> Width/height of region in cells. </summary>
        regionSize : int
        /// <summary> List of cells. </summary>
        cells : Cell list
    }