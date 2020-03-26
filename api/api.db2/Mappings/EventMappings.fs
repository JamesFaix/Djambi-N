namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System
open Apex.Api.Common.Json

[<AutoOpen>]
module EventMappings =

    let toEvent (source : EventSqlModel) : Event =
        raise <| NotImplementedException()

    let toEventSqlModel (source : Event) : EventSqlModel =
        raise <| NotImplementedException()

    let toEventKindSqlId (source : EventKind) : byte =
        raise <| NotImplementedException()

    let createEventRequestToEventSqlModel (source : CreateEventRequest) (kind : EventKindSqlModel) (game : GameSqlModel) (createdBy : UserSqlModel) (actingPlayer : Option<PlayerSqlModel>): EventSqlModel = 
        let x = EventSqlModel()
        x.CreatedOn <- DateTime.UtcNow
        x.Game <- game
        x.ActingPlayer <- actingPlayer |> Option.toObj
        x.CreatedByUser <- createdBy
        x.EffectsJson <- source.effects |> JsonUtility.serialize
        x.Kind <- kind
        x