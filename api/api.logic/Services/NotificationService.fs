namespace Apex.Api.Logic.Services

open System.Collections.Concurrent
open Serilog
open Apex.Api.Common.Control
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open System.Threading.Tasks
open FSharp.Control.Tasks

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

            let otherPlayersUserIds =
                response.game.players
                |> Seq.choose (fun p ->
                    match p.userId with
                    | Some uId when uId <> creatorId -> Some uId
                    | _ -> None
                )

            let removedPlayersUserIds = 
                response.event.effects 
                |> Seq.choose (fun f -> 
                    match f with 
                    | Effect.PlayerRemoved f1 -> f1.oldPlayer.userId 
                    | _ -> None 
                )

            let userIds =
                otherPlayersUserIds
                |> Seq.append removedPlayersUserIds
                |> Seq.distinct
                |> Seq.toList

            subscribers.Values
            |> Seq.filter (fun s -> userIds |> List.contains s.userId)
            |> Seq.map (fun s -> 
                task {
                    try 
                        return! s.send response
                    with
                    | :? HttpException as ex when ex.statusCode = 500 ->
                        (x :> INotificationService).remove s.userId
                        return ()                        
                } :> Task
            )
            |> Task.WhenAll
            |> Task.toGeneric