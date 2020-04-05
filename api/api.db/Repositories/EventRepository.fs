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
open System
open Newtonsoft.Json

type EventRepository(context : ApexDbContext) =
    let playerNameTakenMessage = 
        "The instance of entity type 'PlayerSqlModel' cannot be tracked because another " + 
        "instance with the same key value for {'GameId', 'Name'} is already being tracked."

    interface IEventRepository with
        member __.getEvents (gameId, query) =
            // Unassigned and all invalid values count as Ascending
            let isAscending = query.direction <> ListSortDirection.Descending 

            // Note: LINQ-to-SQL below
            // Using wordy conditional building of IQueryable rather than higher-order-functions
            // because each lambda is an Expression<Func> not a Func

            task {
                let maxResults = 
                    let defaultMaxResults = 10000 // TODO: Move to config
                    match query.maxResults with
                    | Some x -> x
                    | None -> defaultMaxResults

                let mutable sqlQuery = 
                    context.Events
                        .Include(fun e -> e.CreatedByUser)
                        .Where(fun e -> e.Game.GameId = gameId)

                // Filters
                match query.thresholdEventId with
                | None -> ()
                | Some(x) -> 
                    sqlQuery <- 
                        if isAscending
                        then sqlQuery.Where(fun e -> e.EventId <= x)
                        else sqlQuery.Where(fun e -> e.EventId >= x)
                
                match query.thresholdTime with
                | None -> ()
                | Some(x) -> 
                    sqlQuery <- 
                        if isAscending 
                        then sqlQuery.Where(fun e -> e.CreatedOn <= x)
                        else sqlQuery.Where(fun e -> e.CreatedOn >= x)

                // Sorting
                sqlQuery <-
                    if isAscending
                    then sqlQuery.OrderBy(fun e -> e.CreatedOn)
                    else sqlQuery.OrderByDescending(fun e -> e.CreatedOn)

                let! sqlModels =
                    sqlQuery
                        .Take(maxResults)
                        .ToListAsync()
                
                return sqlModels |> Seq.map toEvent |> Seq.toList
            }

        member __.persistEvent (request, oldGame, newGame) =
            let gameId = oldGame.id

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

            let addPlayer(player : Player) : Task<Player> =
                task {
                    let p = player |> toPlayerSqlModel
                    p.PlayerId <- 0
                    p.GameId <- gameId

                    // This should really be handled in the logic layer
                    // Putting here because before it was in a stored proc :(
                    if String.IsNullOrEmpty(p.Name)
                    then
                        let! user = context.Users.FindAsync(p.UserId)
                        p.Name <- user.Name

                    let! _ = context.Players.AddAsync(p)
                    return p |> toPlayer
                }

            let removePlayer(playerId : int) : Task<unit> =
                task {
                    let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = gameId && p.PlayerId = playerId)
                    if p = null
                    then raise <| HttpException(404, "Not found.")

                    context.Players.Remove(p) |> ignore
                    return ()
                }

            let updatePlayer(playerSqlModel : PlayerSqlModel, player : Player) : Task<unit> =
                task {
                    playerSqlModel.PlayerStatusId <- player.status
                    playerSqlModel.ColorId <- player.colorId |> Option.map byte |> Option.toNullable
                    playerSqlModel.StartingRegion <- player.startingRegion |> Option.map byte |> Option.toNullable
                    playerSqlModel.StartingTurnNumber <- player.startingTurnNumber |> Option.map byte |> Option.toNullable
                    // Other properties cannot be mutated
                    context.Players.Update(playerSqlModel) |> ignore
                    return ()
                }

            task {
                let! g = context.Games.FindAsync(gameId)
                if g = null
                then raise <| HttpException(404, "Not found.")

                use! transaction = context.Database.BeginTransactionAsync()
                try
                    // Update game
                    g.AllowGuests <- newGame.parameters.allowGuests
                    g.IsPublic <- newGame.parameters.isPublic
                    g.Description <- newGame.parameters.description |> Option.toObj
                    g.RegionCount <- byte newGame.parameters.regionCount
                    g.CurrentTurnJson <- newGame.currentTurn |> JsonConvert.SerializeObject
                    g.TurnCycleJson <- newGame.turnCycle |> JsonConvert.SerializeObject
                    g.PiecesJson <- newGame.pieces |> JsonConvert.SerializeObject
                    g.GameStatusId <- newGame.status
                    context.Games.Update(g) |> ignore

                    // Update players
                    for p in removedPlayers do
                        let! _ = removePlayer(p.id)
                        ()
                    for p in addedPlayers do
                        let! _ = addPlayer(p)
                        ()

                    let! playerSqlModels = context.Players.Where(fun p -> p.GameId = gameId).ToListAsync()
                    let updates = 
                        modifiedPlayers.Join(
                            playerSqlModels, 
                            (fun p -> p.id), 
                            (fun sqlModel -> sqlModel.PlayerId),
                            (fun player sqlModel -> (player, sqlModel)))

                    for (player, sqlModel) in updates do
                        let! _ = updatePlayer(sqlModel, player)
                        ()

                    // Save event               
                    let e = createEventRequestToEventSqlModel request gameId
                    let! _ = context.Events.AddAsync(e)
                
                    let! _ = context.SaveChangesAsync()
                    let! _ = transaction.CommitAsync()

                    // Query updated data (so primary keys are assigned, etc)
                    let! g = 
                        context.Games
                            .Include(fun g -> g.Players)
                            .Include(fun g -> g.CreatedByUser)
                            .SingleOrDefaultAsync(fun g -> g.GameId = newGame.id)

                    let! e = 
                        context.Events
                            .Include(fun e -> e.CreatedByUser)
                            .SingleOrDefaultAsync(fun e1 -> e1.EventId = e.EventId)

                    let response = {
                        game = g |> toGame
                        event = e |> toEvent
                    }

                    return Ok response
                with
                | :? InvalidOperationException as ex when ex.Message.StartsWith(playerNameTakenMessage) ->
                    return Error <| HttpException(409, "Conflict when attempting to write Event.")
            }