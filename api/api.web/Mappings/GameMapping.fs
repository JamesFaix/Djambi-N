namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model
open System.ComponentModel

[<AutoOpen>]
module GameMapping =
    
    let toGameParametersDto (source : GameParameters) : GameParametersDto =
        {
            allowGuests = source.allowGuests
            isPublic = source.isPublic
            description = source.description |> Option.toObj
            regionCount = source.regionCount
        }

    let toGameParameters (source : GameParametersDto) : GameParameters =
        {
            allowGuests = source.allowGuests
            description = source.description |> Option.ofObj
            isPublic = source.isPublic
            regionCount = source.regionCount
        }

    let toPieceKindDto (source : PieceKind) : PieceKindDto =
        match source with
        | PieceKind.Conduit -> PieceKindDto.Conduit
        | PieceKind.Corpse -> PieceKindDto.Corpse
        | PieceKind.Diplomat -> PieceKindDto.Diplomat
        | PieceKind.Hunter -> PieceKindDto.Hunter
        | PieceKind.Reaper -> PieceKindDto.Reaper
        | PieceKind.Scientist -> PieceKindDto.Scientist
        | PieceKind.Thug -> PieceKindDto.Thug

    let toPieceDto (source : Piece) : PieceDto =
        {
            id = source.id
            cellId = source.cellId
            kind = source.kind |> toPieceKindDto
            playerId = source.playerId |> Option.toNullable
            originalPlayerId = source.originalPlayerId
        }

    let toGameStatusDto (source : GameStatus) : GameStatusDto =
        match source with
        | GameStatus.Canceled -> GameStatusDto.Canceled
        | GameStatus.InProgress -> GameStatusDto.InProgress
        | GameStatus.Over -> GameStatusDto.Over
        | GameStatus.Pending -> GameStatusDto.Pending

    let toGameStatus (source : GameStatusDto) : GameStatus =
        match source with
        | GameStatusDto.Canceled -> GameStatus.Canceled
        | GameStatusDto.InProgress -> GameStatus.InProgress
        | GameStatusDto.Over -> GameStatus.Over
        | GameStatusDto.Pending -> GameStatus.Pending
        | _ -> raise <| InvalidEnumArgumentException("source", int source, typeof<GameStatusDto>)

    let toGameDto (source : Game) : GameDto =
        {
            id = source.id
            createdBy = source.createdBy |> toCreationSourceDto
            currentTurn = source.currentTurn |> Option.map toTurnDto |> Option.toObj
            parameters = source.parameters |> toGameParametersDto
            pieces = source.pieces |> List.map toPieceDto
            players = source.players |> List.map toPlayerDto
            status = source.status |> toGameStatusDto
            turnCycle = source.turnCycle        
        }
        