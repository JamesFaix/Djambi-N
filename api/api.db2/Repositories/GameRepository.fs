namespace Apex.Api.Db.Repositories

open System
open Apex.Api.Db.Model
open Apex.Api.Db.Interfaces
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Microsoft.EntityFrameworkCore
open System.Linq
open Apex.Api.Db.Mappings
open System.Threading.Tasks
open Apex.Api.Common.Json

type GameRepository(context : ApexDbContext) =
    interface IGameRepository with
        member __.getGame gameId =
            task {
                let! g = 
                    context.Games
                        .Include(fun g -> g.CreatedByUser)
                        .SingleOrDefaultAsync(fun g -> g.GameId = gameId)
                if g = null
                then raise <| HttpException(404, "Not found.")

                let! ps = context.Players.Where(fun p -> p.GameId = gameId).ToListAsync()

                let result = toGame g ps
                return Ok(result)
            }
            
        [<Obsolete("Only used for tests")>]
        member __.createGame request =
            task {
                let g = request |> toGameSqlModel
                let! _ = context.Games.AddAsync(g)
                let! _ = context.SaveChangesAsync()
                return Ok(g.GameId)
            }
        
        [<Obsolete("Only used for tests")>]
        member __.addPlayer (gameId, request) =
            task {
                let p = PlayerSqlModel()
                p.GameId <- gameId
                p.PlayerKindId <- request.kind
                p.UserId <- request.userId |> Option.toNullable

                let! u = 
                    match request.userId with
                    | Some userId -> context.Users.SingleOrDefaultAsync(fun u -> u.UserId = userId)
                    | None -> Task.FromResult(Unchecked.defaultof<UserSqlModel>)

                if request.name.IsSome 
                then p.Name <- request.name.Value
                else p.Name <- u.Name

                let! _ = context.Players.AddAsync(p)
                let! _ = context.SaveChangesAsync()
                return Ok(p |> toPlayer)
            }
            
        [<Obsolete("Only used for tests")>]
        member __.removePlayer (gameId, playerId) =
            task {
                let! p = context.Players.SingleOrDefaultAsync(fun p -> p.GameId = gameId && p.PlayerId = playerId)
                context.Players.Remove(p) |> ignore
                let! _ = context.SaveChangesAsync()
                return Ok()
            }

        [<Obsolete("Only used for tests")>]
        member __.updateGame game =
            task {
                let! g = context.Games.FindAsync(game.id)
                g.IsPublic <- game.parameters.isPublic
                g.AllowGuests <- game.parameters.allowGuests
                g.Description <- game.parameters.description |> Option.toObj
                g.RegionCount <- byte game.parameters.regionCount
                g.CurrentTurnJson <- game.currentTurn |> JsonUtility.serialize
                g.TurnCycleJson <- game.turnCycle |> JsonUtility.serialize
                g.PiecesJson <- game.pieces |> JsonUtility.serialize
                context.Games.Update(g) |> ignore
                let! _ = context.SaveChangesAsync()
                return Ok()            
            }

        member __.getNeutralPlayerNames () =
            task {
                let! names = 
                    context.NeutralPlayerNames
                        .Select(fun x -> x.Name)
                        .ToListAsync()

                return Ok(names |> Seq.toList)
            }

        member __.createGameAndAddPlayer (gameRequest, playerRequest) =
            task {
                let gameSqlModel = toGameSqlModel gameRequest
                let! _ = context.Games.AddAsync(gameSqlModel)
                let! _ = context. SaveChangesAsync()

                let playerSqlModel = createPlayerRequestToPlayerSqlModel playerRequest
                playerSqlModel.GameId <- gameSqlModel.GameId
                let! _ = context.Players.AddAsync(playerSqlModel)

                let! _ = context.SaveChangesAsync()

                return Ok(gameSqlModel.GameId)
            }
