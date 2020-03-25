namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module BoardMappings =

    let toLocationDto (source : Location) : LocationDto =
        {
            region = source.region
            x = source.x
            y = source.y
        }

    let toCellDto (source : Cell) : CellDto =
        {
            id = source.id
            locations = source.locations |> List.map toLocationDto
        }

    let toBoardDto (source : Board) : BoardDto =
        {
            regionCount = source.regionCount
            regionSize = source.regionSize
            cells = source.cells |> List.map toCellDto
        }