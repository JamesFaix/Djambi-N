namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module BoardMappings =

    let toLocationDto (source : Location) : LocationDto =
        let result = LocationDto()
        result.Region <- source.region
        result.X <- source.x
        result.Y <- source.y
        result

    let toCellDto (source : Cell) : CellDto =
        let result = CellDto()
        result.Id <- source.id
        result.Locations <-
            source.locations
            |> List.map toLocationDto
            |> List.toArray
        result

    let toBoardDto (source : Board) : BoardDto =
        let result = BoardDto()
        result.Cells <- 
            source.cells 
            |> List.map toCellDto
            |> List.toArray
        result.RegionCount <- source.regionCount
        result.RegionSize <- source.regionSize
        result

    let toPathsDto (source : List<List<int>>) : int[][] =
        source 
        |> List.map List.toArray
        |> List.toArray