namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module SearchMappings =

    let toGamesQuery (source : GamesQueryDto) : GamesQuery =
        {
            gameId = source.gameId |> Option.ofNullable
            descriptionContains = source.descriptionContains |> Option.ofObj
            createdByUserName = source.createdByUserName |> Option.ofObj
            playerUserName = source.playerUserName |> Option.ofObj
            containsMe = source.containsMe |> Option.ofNullable
            isPublic = source.isPublic |> Option.ofNullable
            allowGuests = source.allowGuests |> Option.ofNullable
            statuses = source.statuses |> Seq.map toGameStatus |> Seq.toList
            createdBefore = source.createdBefore |> Option.ofNullable
            createdAfter = source.createdAfter |> Option.ofNullable
            lastEventBefore = source.lastEventBefore |> Option.ofNullable
            lastEventAfter = source.lastEventAfter |> Option.ofNullable
        }

    let toSearchGameDto (source : SearchGame) : SearchGameDto =
        {
            id = source.id
            containsMe = source.containsMe
            createdBy = source.createdBy |> toCreationSourceDto
            lastEventOn = source.lastEventOn
            parameters = source.parameters |> toGameParametersDto
            playerCount = source.playerCount
            status = source.status |> toGameStatusDto
        }