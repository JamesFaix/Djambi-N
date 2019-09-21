namespace Djambi.Api.Logic.Services

open System.Collections.Concurrent
open Serilog
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Interfaces

type NotificationService(log : ILogger) =
    let subscribers = new ConcurrentDictionary<int, ISubscriber>()

    interface INotificationService with
        member x.add subscriber =
            log.Information(sprintf "Notifications: User %i subscribed to notifications using %s" subscriber.userId (subscriber.GetType().Name))
            subscribers.[subscriber.userId] <- subscriber

        member x.remove userId =
            log.Information(sprintf "Notifications: User %i unsubscribed from notifications" userId)
            match subscribers.TryGetValue userId with
            | (true, s) -> 
                s.Dispose()
                subscribers.TryRemove userId
                |> ignore
            | _ -> ()

        member x.send response =
            let creatorId = response.event.createdBy.userId

            let otherUserIds =
                response.game.players
                |> Seq.filter (fun p ->
                    p.userId.IsSome &&
                    p.userId.Value <> creatorId
                )
                |> Seq.map (fun p -> p.userId.Value)
                |> Seq.distinct
                |> Seq.toList

            subscribers.Values
            |> Seq.filter (fun s -> otherUserIds |> List.contains s.userId)
            |> Seq.map (fun s -> 
                s.send response
                |> thenBindErrorAsync 500 (fun err ->
                    (x :> INotificationService).remove s.userId
                    okTask ()
                )
            )
            |> AsyncHttpResult.whenAll