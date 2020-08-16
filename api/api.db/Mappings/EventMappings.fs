namespace Djambi.Api.Db.Mappings

open Djambi.Api.Db.Model
open Djambi.Api.Model
open System
open Newtonsoft.Json

[<AutoOpen>]
module EventMappings =
    
    let toEvent (source : EventSqlModel) : Event =
        {
            id = source.EventId
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            actingPlayerId = source.ActingPlayerId |> Option.ofNullable
            kind = source.EventKindId
            effects = source.EffectsJson |> JsonConvert.DeserializeObject<List<Effect>>
        }

    let toEventSqlModel (source : Event) (gameId : int) : EventSqlModel =
        let x = EventSqlModel()
        x.CreatedOn <- source.createdBy.time
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdBy.userId
        x.EffectsJson <- source.effects |> JsonConvert.SerializeObject
        x.EventKindId <- source.kind
        x
   
    let createEventRequestToEventSqlModel (source : CreateEventRequest) (gameId : int) : EventSqlModel = 
        let x = EventSqlModel()
        x.CreatedOn <- DateTime.UtcNow
        x.GameId <- gameId
        x.ActingPlayerId <- source.actingPlayerId |> Option.toNullable
        x.CreatedByUserId <- source.createdByUserId
        x.EffectsJson <- source.effects |> JsonConvert.SerializeObject
        x.EventKindId <- source.kind
        x