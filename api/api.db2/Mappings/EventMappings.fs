namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System
open Apex.Api.Common.Json
open System.ComponentModel

[<AutoOpen>]
module EventMappings =

    let toEventKind (source : byte) : EventKind =
        match source with
        | 1uy -> EventKind.GameParametersChanged
        | 2uy -> EventKind.GameCanceled 
        | 3uy -> EventKind.PlayerJoined 
        | 4uy -> EventKind.PlayerRemoved 
        | 5uy -> EventKind.GameStarted 
        | 6uy -> EventKind.TurnCommitted 
        | 7uy -> EventKind.TurnReset 
        | 8uy -> EventKind.CellSelected 
        | 9uy -> EventKind.PlayerStatusChanged 
        | _ -> raise <| InvalidEnumArgumentException()

    let toEventKindSqlId (source : EventKind) : byte =
        match source with
        | EventKind.GameParametersChanged -> 1uy
        | EventKind.GameCanceled -> 2uy
        | EventKind.PlayerJoined -> 3uy
        | EventKind.PlayerRemoved -> 4uy
        | EventKind.GameStarted -> 5uy
        | EventKind.TurnCommitted -> 6uy
        | EventKind.TurnReset -> 7uy
        | EventKind.CellSelected -> 8uy
        | EventKind.PlayerStatusChanged -> 9uy

    let toEvent (source : EventSqlModel) : Event =
        {
            id = source.Id
            createdBy = {
                userId = source.CreatedByUser.Id
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            actingPlayerId = source.ActingPlayer |> Option.ofObj |> Option.map (fun x -> x.Id)
            kind = source.KindId |> toEventKind
            effects = source.EffectsJson |> JsonUtility.deserializeList
        }

    let toEventSqlModel (source : Event) (gameId : int) : EventSqlModel =
        let x = EventSqlModel()
        x.CreatedOn <- source.createdBy.time
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdBy.userId
        x.EffectsJson <- source.effects |> JsonUtility.serialize
        x.KindId <- source.kind |> toEventKindSqlId
        x
   
    let createEventRequestToEventSqlModel (source : CreateEventRequest) (gameId : int) : EventSqlModel = 
        let x = EventSqlModel()
        x.CreatedOn <- DateTime.UtcNow
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdByUserId
        x.EffectsJson <- source.effects |> JsonUtility.serialize
        x.KindId <- source.kind |> toEventKindSqlId
        x