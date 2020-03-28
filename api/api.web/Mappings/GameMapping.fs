namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

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

    let toPieceDto (source : Piece) : PieceDto =
        {
            id = source.id
            cellId = source.cellId
            kind = source.kind
            playerId = source.playerId |> Option.toNullable
            originalPlayerId = source.originalPlayerId
        }

    let toGameDto (source : Game) : GameDto =
        {
            id = source.id
            createdBy = source.createdBy |> toCreationSourceDto
            currentTurn = source.currentTurn |> Option.map toTurnDto |> Option.toObj
            parameters = source.parameters |> toGameParametersDto
            pieces = source.pieces |> List.map toPieceDto
            players = source.players |> List.map toPlayerDto
            status = source.status
            turnCycle = source.turnCycle        
        }
        