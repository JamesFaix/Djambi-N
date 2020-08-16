namespace Djambi.Api.Web.Mappings

open Djambi.Api.Model
open Djambi.Api.Web.Model

[<AutoOpen>]
module PlayerMappings =

    let toPlayerDto (source : Player) : PlayerDto =
        {
            id = source.id
            gameId = source.gameId
            name = source.name
            kind = source.kind
            status = source.status
            userId = source.userId |> Option.toNullable
            colorId = source.colorId |> Option.toNullable
            startingRegion = source.startingRegion |> Option.toNullable
            startingTurnNumber = source.startingTurnNumber |> Option.toNullable
        }

    let toCreatePlayerRequest (source : CreatePlayerRequestDto) : CreatePlayerRequest =
        {
            kind = source.kind
            name = source.name |> Option.ofObj
            userId = source.userId |> Option.ofNullable
        }