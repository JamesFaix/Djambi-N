namespace Apex.Api.Web.Mappings

open Apex.Api.Model
open Apex.Api.Web.Model

[<AutoOpen>]
module EventMappings =
    
    let toEventsQuery (source : EventsQueryDto) : EventsQuery =
        {
            maxResults = source.MaxResults |> Option.ofNullable
            direction = source.Direction
            thresholdTime = source.ThresholdTime |> Option.ofNullable  
            thresholdEventId = source.ThresholdEventId |> Option.ofNullable
        }

    let toCurrentTurnChangedEffectDto (source : CurrentTurnChangedEffect) : EffectDto =
        let result = CurrentTurnChangedEffectDto()
        result.NewValue <- source.newValue |> Option.map toTurnDto |> Option.toObj
        result.OldValue <- source.oldValue |> Option.map toTurnDto |> Option.toObj
        result :> EffectDto
        
    let toGameStatusChangedEffectDto (source : GameStatusChangedEffect) : EffectDto =
        let result = GameStatusChangedEffectDto()
        result.NewValue <- source.newValue |> toGameStatusDto
        result.OldValue <- source.oldValue |> toGameStatusDto
        result :> EffectDto

    let toNeutralPlayerAddedEffectDto (source : NeutralPlayerAddedEffect) : EffectDto =
        let result = NeutralPlayerAddedEffectDto()
        result.Name <- source.name
        result.PlaceholderPlayerId <- source.placeholderPlayerId
        result :> EffectDto

    let toParametersChangedEffectDto (source : ParametersChangedEffect) : EffectDto =
        let result = ParametersChangedEffectDto()
        result.NewValue <- source.newValue |> toGameParametersDto
        result.OldValue <- source.oldValue |> toGameParametersDto
        result :> EffectDto
            
    let toPieceAbandonedEffectDto (source : PieceAbandonedEffect) : EffectDto =
        let result = PieceAbandonedEffectDto()
        result.OldPiece <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPieceDroppedEffectDto (source : PieceDroppedEffect) : EffectDto =
        let result = PieceDroppedEffectDto()
        result.NewValue <- source.newPiece |> toPieceDto
        result.OldValue <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPieceEnlistedEffectDto (source : PieceEnlistedEffect) : EffectDto =
        let result = PieceEnlistedEffectDto()
        result.NewPlayerId <- source.newPlayerId
        result.OldPiece <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPieceKilledEffectDto (source : PieceKilledEffect) : EffectDto =
        let result = PieceKilledEffectDto()
        result.OldPiece <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPieceMovedEffectDto (source : PieceMovedEffect) : EffectDto =
        let result = PieceMovedEffectDto()
        result.NewCellId <- source.newCellId
        result.OldPiece <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPieceVacatedEffectDto (source : PieceVacatedEffect) : EffectDto =
        let result = PieceVacatedEffectDto()
        result.NewCellId <- source.newCellId
        result.OldPiece <- source.oldPiece |> toPieceDto
        result :> EffectDto
    
    let toPlayerAddedEffectDto (source : PlayerAddedEffect) : EffectDto =
        let result = PlayerAddedEffectDto()
        result.Name <- source.name |> Option.toObj
        result.PlayerKind <- source.kind |> toPlayerKindDto
        result.UserId <- source.userId
        result :> EffectDto
    
    let toPlayerOutOfMovesEffectDto (source : PlayerOutOfMovesEffect) : EffectDto =
        let result = PlayerOutOfMovesEffectDto()
        result.PlayerId <- source.playerId
        result :> EffectDto
    
    let toPlayerRemovedEffectDto (source : PlayerRemovedEffect) : EffectDto =
        let result = PlayerRemovedEffectDto()
        result.OldPlayer <- source.oldPlayer |> toPlayerDto
        result :> EffectDto

    let toPlayerStatusChangedEffectDto (source : PlayerStatusChangedEffect) : EffectDto =
        let result = PlayerStatusChangedEffectDto()
        result.NewValue <- source.newStatus |> toPlayerStatusDto
        result.OldValue <- source.oldStatus |> toPlayerStatusDto
        result.PlayerId <- source.playerId
        result :> EffectDto
        
    let toTurnCycleAdvancedEffectDto (source : TurnCycleAdvancedEffect) : EffectDto =
        let result = TurnCycleAdvancedEffectDto()
        result.NewValue <- source.newValue |> List.toArray
        result.OldValue <- source.oldValue |> List.toArray
        result :> EffectDto
            
    let toTurnCyclePlayerFellFromPowerEffectDto (source : TurnCyclePlayerFellFromPowerEffect) : EffectDto =
        let result = TurnCyclePlayerFellFromPowerEffectDto()
        result.NewValue <- source.newValue |> List.toArray
        result.OldValue <- source.oldValue |> List.toArray
        result.PlayerId <- source.playerId
        result :> EffectDto
                
    let toTurnCyclePlayerRemovedEffectDto (source : TurnCyclePlayerRemovedEffect) : EffectDto =
        let result = TurnCyclePlayerRemovedEffectDto()
        result.NewValue <- source.newValue |> List.toArray
        result.OldValue <- source.oldValue |> List.toArray
        result.PlayerId <- source.playerId
        result :> EffectDto
                    
    let toTurnCyclePlayerRoseToPowerEffectDto (source : TurnCyclePlayerRoseToPowerEffect) : EffectDto =
        let result = TurnCyclePlayerRoseToPowerEffectDto()
        result.NewValue <- source.newValue |> List.toArray
        result.OldValue <- source.oldValue |> List.toArray
        result.PlayerId <- source.playerId
        result :> EffectDto

    let toEffectDto (source : Effect) : EffectDto =
        match source with
        | CurrentTurnChanged(e) -> toCurrentTurnChangedEffectDto e
        | GameStatusChanged(e) -> toGameStatusChangedEffectDto e
        | NeutralPlayerAdded(e) -> toNeutralPlayerAddedEffectDto e
        | ParametersChanged(e) -> toParametersChangedEffectDto e
        | PieceAbandoned(e) -> toPieceAbandonedEffectDto e
        | PieceDropped(e) -> toPieceDroppedEffectDto e
        | PieceEnlisted(e) -> toPieceEnlistedEffectDto e
        | PieceKilled(e) -> toPieceKilledEffectDto e
        | PieceMoved(e) -> toPieceMovedEffectDto e
        | PieceVacated(e) -> toPieceVacatedEffectDto e
        | PlayerAdded(e) -> toPlayerAddedEffectDto e
        | PlayerOutOfMoves(e) -> toPlayerOutOfMovesEffectDto e
        | Effect.PlayerRemoved(e) -> toPlayerRemovedEffectDto e
        | Effect.PlayerStatusChanged(e) -> toPlayerStatusChangedEffectDto e
        | TurnCycleAdvanced(e) -> toTurnCycleAdvancedEffectDto e
        | TurnCyclePlayerFellFromPower(e) -> toTurnCyclePlayerFellFromPowerEffectDto e
        | TurnCyclePlayerRemoved(e) -> toTurnCyclePlayerRemovedEffectDto e
        | TurnCyclePlayerRoseToPower(e) -> toTurnCyclePlayerRoseToPowerEffectDto e

    let toEventKindDto (source : EventKind) : EventKindDto =
        match source with
        | EventKind.CellSelected -> EventKindDto.CellSelected
        | EventKind.GameCanceled -> EventKindDto.GameCanceled
        | EventKind.GameParametersChanged -> EventKindDto.GameParametersChanged
        | EventKind.GameStarted -> EventKindDto.GameStarted
        | EventKind.PlayerJoined -> EventKindDto.PlayerJoined
        | EventKind.PlayerRemoved -> EventKindDto.PlayerRemoved
        | EventKind.PlayerStatusChanged -> EventKindDto.PlayerStatusChanged
        | EventKind.TurnCommitted -> EventKindDto.TurnCommitted
        | EventKind.TurnReset -> EventKindDto.TurnReset

    let toEventDto (source : Event) : EventDto =
        let result = EventDto()
        result.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        result.CreatedBy <- source.createdBy |> toCreationSourceDto
        result.Effects <- source.effects |> List.map toEffectDto |> List.toArray
        result.Id <- source.id
        result.Kind <- source.kind |> toEventKindDto
        result

    let toEventDtos (source : List<Event>) : EventDto[] =
        source
        |> List.map toEventDto
        |> List.toArray

    let toStateAndEventResponseDto (source : StateAndEventResponse) : StateAndEventResponseDto =
        let result = StateAndEventResponseDto()
        result.Event <- source.event |> toEventDto
        result.Game <- source.game |> toGameDto
        result