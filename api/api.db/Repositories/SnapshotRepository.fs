namespace Apex.Api.Db.Repositories

open System
open System.Linq
open System.Data.Entity.Core
open FSharp.Control.Tasks
open Microsoft.EntityFrameworkCore
open Newtonsoft.Json
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Mappings
open Apex.Api.Db.Model
open Apex.Api.Model

type SnapshotRepository(context : ApexDbContext) =
    interface ISnapshotRepository with
        member __.getSnapshot snapshotId =
            task {
                let! s = context.Snapshots.FindAsync(snapshotId)

                if s = null
                then raise <| ObjectNotFoundException("Snapshot not found.")

                return s |> toSnapshot
            }

        member __.getSnapshotsForGame gameId  =
            task {
                let! sqlModels = context.Snapshots.Where(fun s -> s.Game.GameId = gameId).ToListAsync()
                return sqlModels |> Seq.map toSnapshotInfo |> Seq.toList
            }

        member __.deleteSnapshot snapshotId  =
            task {
                let! s = context.Snapshots.FindAsync(snapshotId)

                if s = null
                then raise <| ObjectNotFoundException("Snapshot not found.")

                context.Snapshots.Remove(s) |> ignore
                let! _ = context.SaveChangesAsync()

                return ()
            }

        member __.createSnapshot request  =
            task {
                let snapshotJson = {
                    game = request.game
                    history = request.history
                }

                let s = SnapshotSqlModel()
                s.Description <- request.description
                s.CreatedByUserId <- request.createdByUserId
                s.GameId <- request.game.id
                s.SnapshotJson <- snapshotJson |> JsonConvert.SerializeObject
                s.CreatedOn <- DateTime.UtcNow
                
                let! _ = context.Snapshots.AddAsync(s)
                let! _ = context.SaveChangesAsync()

                return s.SnapshotId
            }

        member __.loadSnapshot (gameId, snapshotId) =
            task {
                let! s = context.Snapshots.SingleOrDefaultAsync(
                            fun s -> s.SnapshotId = snapshotId && s.Game.GameId = gameId)
                    
                if s = null
                then raise <| ObjectNotFoundException("Snapshot not found.")

                let! gameSqlModel = context.Games.FindAsync(gameId)
                if gameSqlModel = null
                then raise <| ObjectNotFoundException("Game not found.")

                let snapshot = JsonConvert.DeserializeObject<SnapshotJson> s.SnapshotJson

                // Validate player changes
                let! playerSqlModels = context.Players.Where(fun p -> p.Game.GameId = gameId).ToListAsync()
                
                let playersToRemove = 
                    playerSqlModels
                    |> Seq.filter (fun oldPlayer -> 
                        snapshot.game.players 
                        |> Seq.exists(fun newPlayer -> newPlayer.id = oldPlayer.PlayerId) 
                        |> not
                    )
                
                let playersToAdd =
                    snapshot.game.players
                    |> Seq.filter (fun newPlayer ->
                        playerSqlModels
                        |> Seq.exists(fun oldPlayer -> newPlayer.id = oldPlayer.PlayerId)
                        |> not
                    )
                    |> Seq.map toPlayerSqlModel
                    
                if playersToAdd.Any() || playersToRemove.Any()
                then raise <| NotSupportedException("Snapshots can only be used after the game has started.")

                use! transaction = context.Database.BeginTransactionAsync()

                // Remove old history
                let! oldEvents = context.Events.Where(fun e -> e.Game.GameId = gameId).ToArrayAsync()
                context.Events.RemoveRange(oldEvents)

                // Add new history
                let history = 
                    snapshot.history 
                    |> List.map (fun e -> toEventSqlModel e gameId)
                    |> List.toArray
                let! _ = context.Events.AddRangeAsync(history)

                // Modify game
                gameSqlModel.AllowGuests <- snapshot.game.parameters.allowGuests
                gameSqlModel.IsPublic <- snapshot.game.parameters.isPublic
                gameSqlModel.Description <- snapshot.game.parameters.description |> Option.toObj
                gameSqlModel.RegionCount <- byte snapshot.game.parameters.regionCount
                gameSqlModel.GameStatusId <- snapshot.game.status
                gameSqlModel.PiecesJson <- snapshot.game.pieces |> JsonConvert.SerializeObject
                gameSqlModel.TurnCycleJson <- snapshot.game.turnCycle |> JsonConvert.SerializeObject
                gameSqlModel.CurrentTurnJson <- snapshot.game.currentTurn |> JsonConvert.SerializeObject
                context.Games.Update(gameSqlModel) |> ignore

                // Modify players                
                let updatedPlayerSqlModels = snapshot.game.players |> List.map toPlayerSqlModel
                context.Players.UpdateRange(updatedPlayerSqlModels)

                let! _ = context.SaveChangesAsync()
                let! _ = transaction.CommitAsync()

                return ()
            }