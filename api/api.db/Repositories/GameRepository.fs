namespace Djambi.Api.Db.Repositories

open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces

type GameRepository() =
    
    let getGamesWithoutPlayers (query : GamesQuery) : Game List AsyncHttpResult =
        let cmd = Commands2.getGames query
        queryMany<GameSqlModel>(cmd, "Game")
        |> thenMap (List.map Mapping.mapGameResponse)

    let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
        let cmd = Commands2.getGame gameId
        querySingle<GameSqlModel>(cmd, "Game")
        |> thenMap Mapping.mapGameResponse
        
    let getPlayersForGames (gameIds : int list) : Player List AsyncHttpResult =
        let cmd = Commands2.getPlayers gameIds
        queryMany<PlayerSqlModel>(cmd, "Player")
        |> thenMap (List.map Mapping.mapPlayerResponse)

    let getPlayer (gameId : int, playerId : int) : Player AsyncHttpResult =
        let cmd = Commands2.getPlayer (gameId, playerId)
        querySingle<PlayerSqlModel>(cmd, "Player")
        |> thenMap Mapping.mapPlayerResponse
    
    //Exposed for test setup
    member x.updateGame(game : Game) : Unit AsyncHttpResult =
        let cmd = Commands2.updateGame game
        queryUnit(cmd, "Game")

    //Exposed for test setup
    member x.updatePlayer(player : Player) : Unit AsyncHttpResult =
        let cmd = Commands2.updatePlayer player
        queryUnit(cmd, "Player")

    interface IGameRepository with
        member x.getGame gameId = 
            getGameWithoutPlayers gameId
            |> thenBindAsync (fun game -> 
                getPlayersForGames [gameId]
                |> thenMap (fun players -> { game with players = players })
            )

        member x.getGames query = 
            getGamesWithoutPlayers query
            |> thenBindAsync (fun games -> 
                getPlayersForGames (games |> List.map (fun g -> g.id))
                |> thenMap (fun players -> 
                    let playersByGame = players |> List.groupBy (fun p -> p.gameId)
                    games 
                    |> List.map (fun g -> 
                        let playersOpt = playersByGame |> List.tryFind (fun (gameId, _) -> gameId = g.id) 
                        let ps = match playersOpt with 
                                 | Some (_, players) -> players
                                 | _ -> []
                        { g with players = ps}
                    )
                )
            )

        member x.createGame request =
            let cmd = Commands2.createGame request
            querySingle<int>(cmd, "Game")

        member x.addPlayer (gameId, request) =
            let cmd = Commands2.addPendingPlayer (gameId, request)
            querySingle<int>(cmd, "Player")
            |> thenBindAsync (fun pId -> getPlayer (gameId, pId))

        member x.removePlayer (gameId, playerId) =
            let cmd = Commands.removePlayer (gameId, playerId)
            queryUnit(cmd, "Player")

        member x.getNeutralPlayerNames () =
            let cmd = Commands.getNeutralPlayerNames ()
            queryMany<string>(cmd, "Neutral player names")

        member x.createGameAndAddPlayer (gameRequest, playerRequest) =
            task {
                use conn = SqlUtility.getConnection()
                use tran = conn.BeginTransaction()
       
                try 
                    let cmd = Commands2.createGame gameRequest
                              |> CommandDefinition.withTransaction tran
                    let! gameId = conn.QuerySingleAsync<int> cmd
                    let cmd = Commands2.addPendingPlayer (gameId, playerRequest)
                              |> CommandDefinition.withTransaction tran
                    let! _ = conn.ExecuteAsync cmd
                    tran.Commit()
                    return Ok gameId
                with 
                | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
            }