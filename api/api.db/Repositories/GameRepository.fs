namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db.Model
open Djambi.Api.Model

type GameRepository(u : SqlUtility) =
    
    let getGamesWithoutPlayers (query : GamesQuery) : Game List AsyncHttpResult =
        let cmd = Commands2.getGames query
        u.queryMany<GameSqlModel>(cmd, "Game")
        |> thenMap (List.map Mapping.mapGameResponse)

    let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
        let cmd = Commands2.getGame gameId
        u.querySingle<GameSqlModel>(cmd, "Game")
        |> thenMap Mapping.mapGameResponse
        
    let getPlayersForGames (gameIds : int list) : Player List AsyncHttpResult =
        let cmd = Commands2.getPlayers gameIds
        u.queryMany<PlayerSqlModel>(cmd, "Player")
        |> thenMap (List.map Mapping.mapPlayerResponse)

    let getPlayer (gameId : int, playerId : int) : Player AsyncHttpResult =
        let cmd = Commands2.getPlayer (gameId, playerId)
        u.querySingle<PlayerSqlModel>(cmd, "Player")
        |> thenMap Mapping.mapPlayerResponse
    
    //Exposed for test setup
    member x.updateGame(game : Game) : Unit AsyncHttpResult =
        let cmd = Commands2.updateGame game
        u.queryUnit(cmd, "Game")

    //Exposed for test setup
    member x.updatePlayer(player : Player) : Unit AsyncHttpResult =
        let cmd = Commands2.updatePlayer player
        u.queryUnit(cmd, "Player")

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
            u.querySingle<int>(cmd, "Game")

        member x.addPlayer (gameId, request) =
            let cmd = Commands2.addPendingPlayer (gameId, request)
            u.querySingle<int>(cmd, "Player")
            |> thenBindAsync (fun pId -> getPlayer (gameId, pId))

        member x.removePlayer (gameId, playerId) =
            let cmd = Commands.removePlayer (gameId, playerId)
            u.queryUnit(cmd, "Player")

        member x.getNeutralPlayerNames () =
            let cmd = Commands.getNeutralPlayerNames ()
            u.queryMany<string>(cmd, "Neutral player names")

        member x.createGameAndAddPlayer (gameRequest, playerRequest) =
            let cmd1 = Commands2.createGame gameRequest
            let getCmd2 = fun gameId -> Commands2.addPendingPlayer (gameId, playerRequest)
            u.executeTransactionallyButReturnFirstResult<int> (cmd1, getCmd2) "Effect"