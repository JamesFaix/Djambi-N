[<AutoOpen>]
module Djambi.Api.Model.BoardModel

open Djambi.ClientGenerator.Annotations

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

[<ClientType(ClientSection.Board)>]
type Location =
    {
        region : int
        x : int
        y : int
    }

[<ClientType(ClientSection.Board)>]
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

[<ClientType(ClientSection.Board)>]
type Board =
    {
        regionCount : int
        regionSize : int
        cells : Cell list
    }