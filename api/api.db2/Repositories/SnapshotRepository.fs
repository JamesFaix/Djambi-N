namespace Apex.Api.Db.Repositories

open System
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Apex.Api.Db.Mappings
open Microsoft.EntityFrameworkCore
open System.Linq
open Apex.Api.Model
open Apex.Api.Common.Json

type SnapshotRepository(context : ApexDbContext) =
    interface ISnapshotRepository with
        member __.getSnapshot snapshotId =
            task {
                let! s = context.Snapshots.FindAsync(snapshotId)

                if s = null
                then raise <| HttpException(404, "Not found")

                return Ok(s |> toSnapshot)
            }

        member __.getSnapshotsForGame gameId  =
            task {
                let! sqlModels = context.Snapshots.Where(fun s -> s.Game.GameId = gameId).ToListAsync()
                let results = sqlModels |> Seq.map toSnapshotInfo |> Seq.toList
                return Ok(results)
            }

        member __.deleteSnapshot snapshotId  =
            task {
                let! s = context.Snapshots.FindAsync(snapshotId)

                if s = null
                then raise <| HttpException(404, "Not found")

                context.Snapshots.Remove(s) |> ignore
                let! _ = context.SaveChangesAsync()

                return Ok()
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
                s.SnapshotJson <- snapshotJson |> JsonUtility.serialize
                s.CreatedOn <- DateTime.UtcNow
                
                let! _ = context.Snapshots.AddAsync(s)
                let! _ = context.SaveChangesAsync()

                return Ok(s.SnapshotId)
            }

        member __.loadSnapshot (gameId, snapshotId) =
            task {
                let! s = context.Snapshots.SingleOrDefaultAsync(
                            fun s -> s.SnapshotId = snapshotId && s.Game.GameId = gameId)
                    
                if s = null
                then raise <| HttpException(404, "Not found")

                let! gameSqlModel = context.Games.FindAsync(gameId)
                if gameSqlModel = null
                then raise <| HttpException(404, "Not found")

                let snapshot = JsonUtility.deserialize<SnapshotJson> s.SnapshotJson

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
                gameSqlModel.PiecesJson <- snapshot.game.pieces |> JsonUtility.serialize
                gameSqlModel.TurnCycleJson <- snapshot.game.turnCycle |> JsonUtility.serialize
                gameSqlModel.CurrentTurnJson <- snapshot.game.currentTurn |> JsonUtility.serialize
                context.Games.Update(gameSqlModel) |> ignore

                // Modify players                
                let updatedPlayerSqlModels = snapshot.game.players |> List.map toPlayerSqlModel
                context.Players.UpdateRange(updatedPlayerSqlModels)

                let! _ = context.SaveChangesAsync()
                let! _ = transaction.CommitAsync()

                return Ok()
            }