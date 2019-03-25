namespace Djambi.Api.Logic.Services

open System.Collections.Concurrent
open Djambi.Api.Common.Control
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model

type NotificationService() =
    let subscribers = new ConcurrentDictionary<SubscriberId, ISubscriber>()
    
    interface INotificationService with
        member x.add (subscriber : ISubscriber) : unit =
            subscribers.[subscriber.id] <- subscriber

        member x.remove (id : SubscriberId) : unit =
            subscribers.TryRemove id
            |> ignore

        member x.send (response : StateAndEventResponse) : unit AsyncHttpResult =
            let gameId = response.game.id
            let creatorId = response.event.createdByUserId

            let otherUserIds =
                response.game.players
                |> Seq.filter (fun p -> 
                    p.userId.IsSome && 
                    p.userId.Value <> creatorId
                )
                |> Seq.map (fun p -> p.userId.Value)
                |> Seq.toList            

            let subs = 
                subscribers.Values
                |> Seq.filter (fun s -> 
                    match s.id.gameId with
                    //Include subscriptions specifically for this game, except the event creator
                    | Some g -> g = gameId && s.id.userId <> creatorId
                    //Include non-game-specific subscriptions of other users in game
                    | None -> otherUserIds |> List.contains s.id.userId
                )
                //If a user has a game-specific and non-specific subscription, just send to the specific one
                |> Seq.groupBy (fun s -> s.id.userId)
                |> Seq.map (fun (_, subs) -> 
                    let list = subs |> Seq.toList
                    if list |> List.length = 1 
                    then list.Head
                    else 
                        let gameMatch = list |> List.tryFind (fun s -> s.id.gameId.IsSome)
                        match gameMatch with
                        | Some s -> s
                        | None -> list.Head                
                )

            subs
            |> Seq.map (fun s -> s.send response)
            |> AsyncHttpResult.whenAll