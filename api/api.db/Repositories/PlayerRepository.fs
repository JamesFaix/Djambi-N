namespace Apex.Api.Db.Repositories

open System
open System.Data.Entity.Core
open FSharp.Control.Tasks
open Microsoft.EntityFrameworkCore
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Mappings
open Apex.Api.Db.Model
open Apex.Api.Model

type PlayerRepository(context : ApexDbContext) =

    let maybeSave (commit : bool) =
        task {
            if commit
            then 
                let! _ = context.SaveChangesAsync()
                return ()
            else
                return ()
        }

    interface IPlayerRepository with
        member __.addPlayer (gameId, player, ?commit) =
            task {
                let commit = defaultArg commit false

                let p = player |> toPlayerSqlModel
                p.PlayerId <- 0
                p.GameId <- gameId
 
                // TODO: This should really be handled in the logic layer
                // Putting here because before it was in a stored proc :(
                if String.IsNullOrEmpty(p.Name)
                then
                    let! user = context.Users.FindAsync p.UserId
                    p.Name <- user.Name

                let! _ = context.Players.AddAsync p
                let! _ = maybeSave commit
                return p |> toPlayer
            }
            
        member __.removePlayer (gameId, playerId, ?commit) =
            task {
                let commit  = defaultArg commit false

                let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = gameId && p.PlayerId = playerId)
                if p = null
                then raise <| ObjectNotFoundException("Player not found.")

                context.Players.Remove p |> ignore
                let! _ = maybeSave commit
                return ()
            }

                
        member __.updatePlayer (gameId, player, ?commit) =
            task {
                let commit = defaultArg commit false

                let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = gameId && p.PlayerId = player.id)
                if p = null                
                // TODO: Better exception
                then raise <| ObjectNotFoundException("Player not found.")

                p.PlayerStatusId <- player.status
                p.ColorId <- player.colorId |> Option.map byte |> Option.toNullable
                p.StartingRegion <- player.startingRegion |> Option.map byte |> Option.toNullable
                p.StartingTurnNumber <- player.startingTurnNumber |> Option.map byte |> Option.toNullable
                // Other properties cannot be mutated
 
                context.Players.Update p |> ignore
                let! _ = maybeSave commit
                return ()
            }