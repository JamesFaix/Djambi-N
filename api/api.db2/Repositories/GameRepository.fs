namespace Apex.Api.Db.Repositories

open System
open Apex.Api.Db.Model
open Apex.Api.Db.Interfaces
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Microsoft.EntityFrameworkCore
open System.Linq
open Apex.Api.Db.Mappings

type GameRepository(context : ApexDbContext) =
    interface IGameRepository with
        member __.getGame gameId =
            task {
                let! g = context.Games.FindAsync(gameId)
                if g = null
                then raise <| HttpException(404, "Not found.")

                let! ps = context.Players.Where(fun p -> p.GameId = gameId).ToListAsync()

                let result = toGame g ps
                return Ok(result)
            }
            
        [<Obsolete("Only used for tests")>]
        member __.createGame request =
            raise <| NotImplementedException()
        
        [<Obsolete("Only used for tests")>]
        member __.addPlayer (gameId, request) =
            raise <| NotImplementedException()
            
        [<Obsolete("Only used for tests")>]
        member __.removePlayer (gameId, playerId) =
            raise <| NotImplementedException()

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

                let playerSqlModel = createPlayerRequestToPlayerSqlModel playerRequest None
                playerSqlModel.GameId <- gameSqlModel.Id
                let! _ = context.Players.AddAsync(playerSqlModel)

                let! _ = context.SaveChangesAsync()

                return Ok(gameSqlModel.Id)
            }
