namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model
open System.ComponentModel

[<AutoOpen>]
module PlayerMappings =
    let toPlayerStatusDto (source : PlayerStatus) : PlayerStatusDto =
        match source with
        | PlayerStatus.AcceptsDraw -> PlayerStatusDto.AcceptsDraw
        | PlayerStatus.Alive -> PlayerStatusDto.Alive
        | PlayerStatus.Conceded -> PlayerStatusDto.Conceded
        | PlayerStatus.Eliminated -> PlayerStatusDto.Eliminated
        | PlayerStatus.Pending -> PlayerStatusDto.Pending
        | PlayerStatus.Victorious -> PlayerStatusDto.Victorious
        | PlayerStatus.WillConcede -> PlayerStatusDto.WillConcede

    let toPlayerStatus (source : PlayerStatusDto) : PlayerStatus =
        match source with
        | PlayerStatusDto.AcceptsDraw -> PlayerStatus.AcceptsDraw
        | PlayerStatusDto.Alive -> PlayerStatus.Alive
        | PlayerStatusDto.Conceded -> PlayerStatus.Conceded
        | PlayerStatusDto.Eliminated -> PlayerStatus.Eliminated
        | PlayerStatusDto.Pending -> PlayerStatus.Pending
        | PlayerStatusDto.Victorious -> PlayerStatus.Victorious
        | PlayerStatusDto.WillConcede -> PlayerStatus.WillConcede
        | _ -> raise <| InvalidEnumArgumentException("source", int source, typeof<PlayerStatusDto>)

    let toPlayerDto (source : Player) : PlayerDto =
        let result = PlayerDto()
        result.ColorId <- source.colorId |> Option.toNullable
        result.GameId <- source.gameId
        result.Id <- source.id
        result.Name <- source.name
        result.StartingRegion <- source.startingRegion |> Option.toNullable
        result.StartingTurnNumber <- source.startingTurnNumber |> Option.toNullable
        result.Status <- source.status |> toPlayerStatusDto
        result.UserId <- source.userId |> Option.toNullable
        result

    let toPlayerKind (source : PlayerKindDto) : PlayerKind =
        match source with
        | PlayerKindDto.Guest -> PlayerKind.Guest
        | PlayerKindDto.Neutral -> PlayerKind.Neutral
        | PlayerKindDto.User -> PlayerKind.User
        | _ -> raise <| InvalidEnumArgumentException("source", int source, typeof<PlayerKindDto>)
        
    let toPlayerKindDto (source : PlayerKind) : PlayerKindDto =
        match source with
        | PlayerKind.Guest -> PlayerKindDto.Guest
        | PlayerKind.Neutral -> PlayerKindDto.Neutral
        | PlayerKind.User -> PlayerKindDto.User

    let toCreatePlayerRequest (source : CreatePlayerRequestDto) : CreatePlayerRequest =
        {
            kind = source.Kind |> toPlayerKind
            name = source.Name |> Option.ofObj
            userId = source.UserId |> Option.ofNullable
        }