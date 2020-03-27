namespace Apex.Api.Db.Repositories

open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open System.Linq
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks
open Apex.Api.Db.Mappings
open Apex.Api.Model
open System.ComponentModel
open Apex.Api.Common.Control
open System.Threading.Tasks
open Apex.Api.Common.Json

type EventRepository(context : ApexDbContext) =

    interface IEventRepository with
        member __.getEvents (gameId, query) =
            let noneOr (f : 'a -> bool) (o : Option<'a>)  =
                o.IsNone || f(o.Value)

            let orderBy (d : ListSortDirection) (f : 'a -> 'b) (xs : IQueryable<'a>) : IOrderedQueryable<'a> =
                if d = ListSortDirection.Descending
                then xs.OrderByDescending(f)
                else xs.OrderBy(f)

            let queryFilter (query : EventsQuery) (event : EventSqlModel) : bool =
                let isAscending = query.direction <> ListSortDirection.Descending

                query.thresholdEventId |> noneOr (fun x -> 
                    if isAscending 
                    then event.EventId <= x 
                    else event.EventId >= x
                ) &&
                query.thresholdTime |> noneOr (fun x -> 
                    if isAscending 
                    then event.CreatedOn <= x
                    else event.CreatedOn >= x
                )

            task {
                let maxResults = 
                    match query.maxResults with
                    | Some x -> x
                    | None -> 10000 // TODO: Move to config

                let mutable sqlQuery = 
                    context.Events
                        .Where(fun e -> e.Game.GameId = gameId)
                        .Where(queryFilter query)

                sqlQuery <- sqlQuery |> orderBy query.direction (fun e -> e.CreatedOn)

                let! sqlModels =
                    sqlQuery
                        .Take(maxResults)
                        .ToListAsync()
                
                return Ok(sqlModels |> Seq.map toEvent |> Seq.toList)
            }

        member __.persistEvent (request, oldGame, newGame) =
            // Find players to update
            let removedPlayers =
                oldGame.players
                |> Seq.filter (fun oldP ->
                    newGame.players
                    |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))

            let addedPlayers =
                newGame.players
                |> Seq.filter (fun newP ->
                    oldGame.players
                    |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))

            let modifiedPlayers =
                Enumerable.Join(
                    oldGame.players, newGame.players,
                    (fun p -> p.id), (fun p -> p.id),
                    (fun oldP newP -> (oldP, newP))
                )
                |> Seq.filter (fun (o, n) -> o <> n)
                |> Seq.map (fun (_, n) -> n)

            let addPlayer(game : GameSqlModel, player : Player) : Task<Player> =
                task {
                    let p = player |> toPlayerSqlModel
                    p.Game <- game

                    let! _ = context.Players.AddAsync(p)
                    return p |> toPlayer
                }

            let removePlayer(game : GameSqlModel, playerId : int) : Task<unit> =
                task {
                    let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = game.GameId && p.PlayerId = playerId)
                    if p = null
                    then raise <| HttpException(404, "Not found.")

                    context.Players.Remove(p) |> ignore
                    return ()
                }

            let updatePlayer(game : GameSqlModel, player : Player) : Task<unit> =
                task {
                    let p = player |> toPlayerSqlModel
                    p.Game <- game

                    context.Players.Update(p) |> ignore
                    return ()
                }

            task {
                let! g = context.Games.FindAsync(oldGame.id)
                if g = null
                then raise <| HttpException(404, "Not found.")

                // Update game
                g.AllowGuests <- newGame.parameters.allowGuests
                g.IsPublic <- newGame.parameters.isPublic
                g.Description <- newGame.parameters.description |> Option.toObj
                g.RegionCount <- byte newGame.parameters.regionCount
                g.CurrentTurnJson <- newGame.currentTurn |> JsonUtility.serialize
                g.TurnCycleJson <- newGame.turnCycle |> JsonUtility.serialize
                g.PiecesJson <- newGame.pieces |> JsonUtility.serialize
                g.GameStatusId <- newGame.status |> toGameStatusSqlId
                context.Games.Update(g) |> ignore

                // Update players
                for p in removedPlayers do
                    let! _ = removePlayer(g, p.id)
                    ()
                for p in addedPlayers do
                    let! _ = addPlayer(g, p)
                    ()
                for p in modifiedPlayers do
                    let! _ = updatePlayer(g, p)
                    ()

                // Save event               
                let e = createEventRequestToEventSqlModel request g.GameId
                let! _ = context.Events.AddAsync(e)
                
                let! _ = context.SaveChangesAsync()

                let response = {
                    game = newGame
                    event = e |> toEvent
                }

                return Ok response
            }