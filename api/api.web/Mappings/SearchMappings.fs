namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module SearchMappings =

    let toGamesQuery (source : GamesQueryDto) : GamesQuery =
        {
            gameId = source.GameId |> Option.ofNullable
            descriptionContains = source.DescriptionContains |> Option.ofObj
            createdByUserName = source.CreatedByUserName |> Option.ofObj
            playerUserName = source.PlayerUserName |> Option.ofObj
            containsMe = source.ContainsMe |> Option.ofNullable
            isPublic = source.IsPublic |> Option.ofNullable
            allowGuests = source.AllowGuests |> Option.ofNullable
            statuses = source.Statuses |> Seq.map toGameStatus |> Seq.toList
            createdBefore = source.CreatedBefore |> Option.ofNullable
            createdAfter = source.CreatedAfter |> Option.ofNullable
            lastEventBefore = source.LastEventBefore |> Option.ofNullable
            lastEventAfter = source.LastEventAfter |> Option.ofNullable
        }

    let toSearchGameDto (source : SearchGame) : SearchGameDto =
        let result = SearchGameDto()
        result.ContainsMe <- source.containsMe
        result.CreatedBy <- source.createdBy |> toCreationSourceDto
        result.Id <- source.id
        result.LastEventOn <- source.lastEventOn
        result.Parameters <- source.parameters |> toGameParametersDto
        result.PlayerCount <- source.playerCount
        result.Status <- source.status |> toGameStatusDto
        result

    let toSearchGameDtos (source : List<SearchGame>) : SearchGameDto[] =
        source
        |> List.map toSearchGameDto
        |> List.toArray