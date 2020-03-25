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

    let toPlayerDto (source : Player) : PlayerDto =
        {
            id = source.id
            gameId = source.gameId
            name = source.name
            kind = source.kind |> toPlayerKindDto
            status = source.status |> toPlayerStatusDto
            userId = source.userId |> Option.toNullable
            colorId = source.colorId |> Option.toNullable
            startingRegion = source.startingRegion |> Option.toNullable
            startingTurnNumber = source.startingTurnNumber |> Option.toNullable
        }

    let toCreatePlayerRequest (source : CreatePlayerRequestDto) : CreatePlayerRequest =
        {
            kind = source.kind |> toPlayerKind
            name = source.name |> Option.ofObj
            userId = source.userId |> Option.ofNullable
        }