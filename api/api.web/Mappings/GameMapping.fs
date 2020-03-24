namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model
open System.ComponentModel

[<AutoOpen>]
module GameMapping =
    
    let toGameParametersDto (source : GameParameters) : GameParametersDto =
        let result = GameParametersDto()
        result.AllowGuests <- source.allowGuests
        result.Description <- source.description |> Option.toObj
        result.IsPublic <- source.isPublic
        result.RegionCount <- source.regionCount
        result

    let toGameParameters (source : GameParametersDto) : GameParameters =
        {
            allowGuests = source.AllowGuests
            description = source.Description |> Option.ofObj
            isPublic = source.IsPublic
            regionCount = source.RegionCount
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
        let result = PieceDto()
        result.CellId <- source.cellId
        result.Id <- source.id
        result.Kind <- source.kind |> toPieceKindDto
        result.OriginalPlayerId <- source.originalPlayerId
        result.PlayerId <- source.playerId |> Option.toNullable
        result

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
        let result = GameDto()
        result.CreatedBy <- source.createdBy |> toCreationSourceDto
        result.CurrentTurn <- 
            source.currentTurn 
            |> Option.map toTurnDto
            |> Option.toObj
        result.Id <- source.id
        result.Parameters <- source.parameters |> toGameParametersDto
        result.Pieces <-
            source.pieces
            |> List.map toPieceDto
            |> List.toArray            
        result.Players <-
            source.players
            |> List.map toPlayerDto
            |> List.toArray
        result.Status <- source.status |> toGameStatusDto
        result.TurnCycle <- source.turnCycle |> List.toArray
        result