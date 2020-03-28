namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System
open Apex.Api.Common.Json
open System.ComponentModel
open System.Collections.Generic

[<AutoOpen>]
module EventMappings =
    let private eventKinds = HashSet<(EventKind * EventKindSqlId)>([
        (EventKind.GameParametersChanged, EventKindSqlId.GameParametersChanged)
        (EventKind.GameCanceled, EventKindSqlId.GameCanceled)
        (EventKind.PlayerJoined, EventKindSqlId.PlayerJoined)
        (EventKind.PlayerRemoved, EventKindSqlId.PlayerRemoved)
        (EventKind.GameStarted, EventKindSqlId.GameStarted)
        (EventKind.TurnCommitted, EventKindSqlId.TurnCommitted)
        (EventKind.TurnReset, EventKindSqlId.TurnReset)
        (EventKind.CellSelected, EventKindSqlId.CellSelected)
        (EventKind.PlayerStatusChanged, EventKindSqlId.PlayerStatusChanged)
    ])

    let toEventKind (source : EventKindSqlId) : EventKind =
        let (kind, _) = eventKinds |> Seq.find (fun (_, id) -> id = source)
        kind

    let toEventKindSqlId (source : EventKind) : EventKindSqlId =
        let (_, id) = eventKinds |> Seq.find (fun (kind, _) -> kind = source)
        id

    let toEvent (source : EventSqlModel) : Event =
        {
            id = source.EventId
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            actingPlayerId = source.ActingPlayer |> Option.ofObj |> Option.map (fun x -> x.PlayerId)
            kind = source.EventKindId |> toEventKind
            effects = source.EffectsJson |> JsonUtility.deserializeList
        }

    let toEventSqlModel (source : Event) (gameId : int) : EventSqlModel =
        let x = EventSqlModel()
        x.CreatedOn <- source.createdBy.time
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdBy.userId
        x.EffectsJson <- source.effects |> JsonUtility.serialize
        x.EventKindId <- source.kind |> toEventKindSqlId
        x
   
    let createEventRequestToEventSqlModel (source : CreateEventRequest) (gameId : int) : EventSqlModel = 
        let x = EventSqlModel()
        x.CreatedOn <- DateTime.UtcNow
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdByUserId
        x.EffectsJson <- source.effects |> JsonUtility.serialize
        x.EventKindId <- source.kind |> toEventKindSqlId
        x