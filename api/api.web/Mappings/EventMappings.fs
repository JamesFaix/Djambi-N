namespace Djambi.Api.Web.Mappings

open Djambi.Api.Model
open Djambi.Api.Web.Model
open Djambi.Api.Enums

[<AutoOpen>]
module EventMappings =
    
    let toEventsQuery (source : EventsQueryDto) : EventsQuery =
        {
            maxResults = source.maxResults |> Option.ofNullable
            direction = source.direction
            thresholdTime = source.thresholdTime |> Option.ofNullable  
            thresholdEventId = source.thresholdEventId |> Option.ofNullable
        }

    let toCurrentTurnChangedEffectDto (source : CurrentTurnChangedEffect) : EffectDto =
        ChangeEffectDto<TurnDto>(
            EffectKind.CurrentTurnChanged,
            source.oldValue |> Option.map toTurnDto |> Option.toObj,
            source.newValue |> Option.map toTurnDto |> Option.toObj
        ) :> EffectDto
        
    let toGameStatusChangedEffectDto (source : GameStatusChangedEffect) : EffectDto =
        ChangeEffectDto<GameStatus>(
            EffectKind.GameStatusChanged,
            source.oldValue,
            source.newValue
        ) :> EffectDto

    let toNeutralPlayerAddedEffectDto (source : NeutralPlayerAddedEffect) : EffectDto =
        NeutralPlayerAddedEffectDto(source.name, source.placeholderPlayerId) :> EffectDto

    let toParametersChangedEffectDto (source : ParametersChangedEffect) : EffectDto =
        ChangeEffectDto<GameParametersDto>(
            EffectKind.ParametersChanged,
            source.oldValue |> toGameParametersDto,
            source.newValue |> toGameParametersDto
        ) :> EffectDto
            
    let toPieceAbandonedEffectDto (source : PieceAbandonedEffect) : EffectDto =
        PieceUpdateEffectDto(EffectKind.PieceAbandoned, source.oldPiece |> toPieceDto) :> EffectDto
    
    let toPieceDroppedEffectDto (source : PieceDroppedEffect) : EffectDto =
        ChangeEffectDto<PieceDto>(
            EffectKind.PieceDropped,
            source.oldPiece |> toPieceDto,
            source.newPiece |> toPieceDto
        ) :> EffectDto
    
    let toPieceEnlistedEffectDto (source : PieceEnlistedEffect) : EffectDto =
        PieceEnlistedEffectDto(source.oldPiece |> toPieceDto, source.newPlayerId) :> EffectDto
    
    let toPieceKilledEffectDto (source : PieceKilledEffect) : EffectDto =
        PieceUpdateEffectDto(EffectKind.PieceKilled, source.oldPiece |> toPieceDto) :> EffectDto
    
    let toPieceMovedEffectDto (source : PieceMovedEffect) : EffectDto =
        PieceMovedEffectDto(source.oldPiece |> toPieceDto, source.newCellId) :> EffectDto
    
    let toPieceVacatedEffectDto (source : PieceVacatedEffect) : EffectDto =
        PieceVacatedEffectDto(source.oldPiece |> toPieceDto, source.newCellId) :> EffectDto
    
    let toPlayerAddedEffectDto (source : PlayerAddedEffect) : EffectDto =
        PlayerAddedEffectDto(
            source.name |> Option.toObj, 
            source.userId, 
            source.kind
        ) :> EffectDto

    let toPlayerOutOfMovesEffectDto (source : PlayerOutOfMovesEffect) : EffectDto =
        PlayerOutOfMovesEffectDto(source.playerId) :> EffectDto
    
    let toPlayerRemovedEffectDto (source : PlayerRemovedEffect) : EffectDto =
        PlayerRemovedEffectDto(source.oldPlayer |> toPlayerDto) :> EffectDto

    let toPlayerStatusChangedEffectDto (source : PlayerStatusChangedEffect) : EffectDto =
        PlayerStatusChangedEffectDto(
            source.oldStatus,
            source.newStatus,
            source.playerId
        ) :> EffectDto
        
    let toTurnCycleAdvancedEffectDto (source : TurnCycleAdvancedEffect) : EffectDto =
        ChangeEffectDto<List<int>>(EffectKind.TurnCycleAdvanced, source.oldValue, source.newValue) :> EffectDto
            
    let toTurnCyclePlayerFellFromPowerEffectDto (source : TurnCyclePlayerFellFromPowerEffect) : EffectDto =
        TurnCyclePlayerFellFromPowerEffectDto(source.oldValue, source.newValue, source.playerId) :> EffectDto
                
    let toTurnCyclePlayerRemovedEffectDto (source : TurnCyclePlayerRemovedEffect) : EffectDto =
        TurnCyclePlayerRemovedEffectDto(source.oldValue, source.newValue, source.playerId) :> EffectDto
                    
    let toTurnCyclePlayerRoseToPowerEffectDto (source : TurnCyclePlayerRoseToPowerEffect) : EffectDto =
        TurnCyclePlayerRoseToPowerEffectDto(source.oldValue, source.newValue, source.playerId) :> EffectDto

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

    let toEventDto (source : Event) : EventDto =
        {
            id = source.id
            createdBy =  source.createdBy |> toCreationSourceDto
            actingPlayerId = source.actingPlayerId |> Option.toNullable
            effects = source.effects |> List.map toEffectDto
            kind = source.kind
        }

    let toEventDtos (source : List<Event>) : EventDto[] =
        source
        |> List.map toEventDto
        |> List.toArray

    let toStateAndEventResponseDto (source : StateAndEventResponse) : StateAndEventResponseDto =
        {
            event = source.event |> toEventDto
            game = source.game |> toGameDto
        }