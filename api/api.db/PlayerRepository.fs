namespace Apex.Api.Db.Repositories

open System
open FSharp.Control.Tasks
open Microsoft.EntityFrameworkCore
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open Apex.Api.Db.Mappings
open Apex.Api.Model

type PlayerRepository(context : ApexDbContext) =
    interface IPlayerRepository with
        member __.addPlayer (gameId, player) =
            task {
                let p = player |> toPlayerSqlModel
                p.PlayerId <- 0
                p.GameId <- gameId
 
                // TODO: This should really be handled in the logic layer
                // Putting here because before it was in a stored proc :(
                if String.IsNullOrEmpty(p.Name)
                then
                    let! user = context.Users.FindAsync(p.UserId)
                    p.Name <- user.Name

                let! _ = context.Players.AddAsync(p)
                // TODO: Save?
                return p |> toPlayer
            }
            
        member __.removePlayer (gameId, playerId) =
            task {
                let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = gameId && p.PlayerId = playerId)
                if p = null
                // TODO: Better exception
                then raise <| HttpException(404, "Not found.")

                context.Players.Remove(p) |> ignore

                // TODO: Save?
                return ()
            }

                
        member __.updatePlayer (gameId, player) =
            task {
                let! p = context.Players.SingleOrDefaultAsync(fun p -> p.Game.GameId = gameId && p.PlayerId = player.id)
                if p = null                
                // TODO: Better exception
                then raise <| HttpException(404, "Not found.")

                p.PlayerStatusId <- player.status
                p.ColorId <- player.colorId |> Option.map byte |> Option.toNullable
                p.StartingRegion <- player.startingRegion |> Option.map byte |> Option.toNullable
                p.StartingTurnNumber <- player.startingTurnNumber |> Option.map byte |> Option.toNullable
                // Other properties cannot be mutated
                context.Players.Update(p) |> ignore
                // TODO: Save?
                return ()
            }