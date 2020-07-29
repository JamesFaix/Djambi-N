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
open Apex.Api.Enums
open Newtonsoft.Json

type GameRepository(context : ApexDbContext) =
    let nameConflictMessage = 
        "The instance of entity type 'PlayerSqlModel' cannot be tracked because " + 
        "another instance with the same key value for {'GameId', 'Name'} is already being tracked."

    interface IGameRepository with
        member __.getGame gameId =
            task {
                let! g = 
                    context.Games
                        .Include(fun g -> g.Players)
                        .Include(fun g -> g.CreatedByUser)
                        .SingleOrDefaultAsync(fun g -> g.GameId = gameId)
                if g = null
                then return raise <| HttpException(404, "Game not found.")
                else 
                    g.Players <- g.Players.OrderBy(fun p -> p.PlayerId).ToList()
                    return g |> toGame
            }
            
        [<Obsolete("Only used for tests")>]
        member __.createGame request =
            task {
                let g = request |> toGameSqlModel
                let! _ = context.Games.AddAsync(g)
                let! _ = context.SaveChangesAsync()
                return g.GameId
            }
        
        [<Obsolete("Only used for tests")>]
        member __.updateGame game =
            task {
                let! g = context.Games.FindAsync(game.id)
                g.IsPublic <- game.parameters.isPublic
                g.AllowGuests <- game.parameters.allowGuests
                g.Description <- game.parameters.description |> Option.toObj
                g.RegionCount <- byte game.parameters.regionCount
                g.GameStatusId <- game.status
                g.CurrentTurnJson <- game.currentTurn |> JsonConvert.SerializeObject
                g.TurnCycleJson <- game.turnCycle |> JsonConvert.SerializeObject
                g.PiecesJson <- game.pieces |> JsonConvert.SerializeObject
                context.Games.Update(g) |> ignore
                let! _ = context.SaveChangesAsync()
                return ()            
            }

        member __.getNeutralPlayerNames () =
            task {
                let! names = 
                    context.NeutralPlayerNames
                        .Select(fun x -> x.Name)
                        .ToListAsync()

                return names |> Seq.toList
            }

        member __.createGameAndAddPlayer (gameRequest, playerRequest) =
            task {
                use! transaction = context.Database.BeginTransactionAsync()

                let gameSqlModel = toGameSqlModel gameRequest
                let! _ = context.Games.AddAsync(gameSqlModel)
                let! _ = context. SaveChangesAsync()

                let playerSqlModel = createPlayerRequestToPlayerSqlModel playerRequest
                playerSqlModel.GameId <- gameSqlModel.GameId
                let! _ = context.Players.AddAsync(playerSqlModel)

                let! _ = context.SaveChangesAsync()
                let! _ = transaction.CommitAsync()

                return gameSqlModel.GameId
            }
