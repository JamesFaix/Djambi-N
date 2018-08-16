namespace Djambi.Api.Domain

module BoardModels =
    
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