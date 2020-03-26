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
                let! sqlModels = context.Snapshots.Where(fun s -> s.Game.Id = gameId).ToListAsync()
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
                let! u = context.Users.FindAsync(request.createdByUserId)
                if u = null
                then raise <| HttpException(404, "Not found")

                let! g = context.Games.FindAsync(request.game.id)
                if g = null
                then raise <| HttpException(404, "Not found")

                let snapshotJson = {
                    game = request.game
                    history = request.history
                }

                let json = snapshotJson |> JsonUtility.serialize

                let s = SnapshotSqlModel()
                s.Description <- request.description
                s.CreatedByUser <- u
                s.Game <- g
                s.SnapshotJson <- json
                s.CreatedOn <- DateTime.UtcNow
                
                let! _ = context.Snapshots.AddAsync(s)
                let! _ = context.SaveChangesAsync()

                return Ok(s.Id)
            }

        member __.loadSnapshot (gameId, snapshotId)  =
            task {
                let! s = context.Snapshots.SingleOrDefaultAsync(
                            fun s -> s.Id = snapshotId && s.Game.Id = gameId)
                    
                if s = null
                then raise <| HttpException(404, "Not found")

                let! gameSqlModel = context.Games.FindAsync(gameId)
                if gameSqlModel = null
                then raise <| HttpException(404, "Not found")

                let snapshot = JsonUtility.deserialize<SnapshotJson> s.SnapshotJson

                // Validate player changes
                let! playerSqlModels = context.Players.Where(fun p -> p.Game.Id = gameId).ToListAsync()
                
                let playersToRemove = 
                    playerSqlModels
                    |> Seq.filter (fun oldPlayer -> 
                        snapshot.game.players 
                        |> Seq.exists(fun newPlayer -> newPlayer.id = oldPlayer.Id) 
                        |> not
                    )
                
                let playersToAdd =
                    snapshot.game.players
                    |> Seq.filter (fun newPlayer ->
                        playerSqlModels
                        |> Seq.exists(fun oldPlayer -> newPlayer.id = oldPlayer.Id)
                        |> not
                    )
                    |> Seq.map toPlayerSqlModel
                    
                if playersToAdd.Any() || playersToRemove.Any()
                then raise <| NotSupportedException("Snapshots can only be used after the game has started.")

                // Remove old history
                let! oldEvents = context.Events.Where(fun e -> e.Game.Id = gameId).ToArrayAsync()
                context.Events.RemoveRange(oldEvents)

                // Add new history
                let history = snapshot.history |> List.map toEventSqlModel |> List.toArray
                let! _ = context.Events.AddRangeAsync(history)

                let! status = context.GameStatuses.FindAsync(snapshot.game.status |> toGameStatusSqlId)
                if status = null
                then raise <| HttpException(404, "Not found")

                // Modify game
                gameSqlModel.AllowGuests <- snapshot.game.parameters.allowGuests
                gameSqlModel.IsPublic <- snapshot.game.parameters.isPublic
                gameSqlModel.Description <- snapshot.game.parameters.description |> Option.toObj
                gameSqlModel.RegionCount <- byte snapshot.game.parameters.regionCount
                gameSqlModel.Status <- status
                gameSqlModel.PiecesJson <- snapshot.game.pieces |> JsonUtility.serialize
                gameSqlModel.TurnCycleJson <- snapshot.game.turnCycle |> JsonUtility.serialize
                gameSqlModel.CurrentTurnJson <- snapshot.game.currentTurn |> JsonUtility.serialize

                // Modify players                
                let updatedPlayerSqlModels = snapshot.game.players |> List.map toPlayerSqlModel
                context.Players.UpdateRange(updatedPlayerSqlModels)

                let! _ = context.SaveChangesAsync()

                return Ok()
            }