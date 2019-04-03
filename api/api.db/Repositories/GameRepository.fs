namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model
open System

type GameRepository(ctxProvider : CommandContextProvider) =
    
    let getGamesWithoutPlayers (query : GamesQuery) : Game List AsyncHttpResult =
        Commands2.getGames query
        |> Command.execute ctxProvider
        |> thenMap (List.map Mapping.mapGameResponse)

    let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
        Commands2.getGame gameId
        |> Command.execute ctxProvider
        |> thenMap Mapping.mapGameResponse
        
    let getPlayersForGames (gameIds : int list) : Player List AsyncHttpResult =
        Commands2.getPlayers gameIds
        |> Command.execute ctxProvider
        |> thenMap (List.map Mapping.mapPlayerResponse)

    let getPlayer (gameId : int, playerId : int) : Player AsyncHttpResult =
        Commands2.getPlayer (gameId, playerId)
        |> Command.execute ctxProvider
        |> thenMap Mapping.mapPlayerResponse
    
    //Exposed for test setup
    member x.updateGame(game : Game) : Unit AsyncHttpResult =
        Commands2.updateGame game
        |> Command.execute ctxProvider

    //Exposed for test setup
    member x.updatePlayer(player : Player) : Unit AsyncHttpResult =
        Commands2.updatePlayer player
        |> Command.execute ctxProvider

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
                match games with
                | [] -> okTask []
                | _ ->
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

        [<Obsolete>]
        member x.createGame request =
            Commands2.createGame request
            |> Command.execute ctxProvider

        [<Obsolete>]
        member x.addPlayer (gameId, request) =
            Commands2.addPendingPlayer (gameId, request)
            |> Command.execute ctxProvider
            |> thenBindAsync (fun pId -> getPlayer (gameId, pId))

        [<Obsolete>]
        member x.removePlayer (gameId, playerId) =
            Commands.removePlayer (gameId, playerId)
            |> Command.execute ctxProvider

        member x.getNeutralPlayerNames () =
            Commands.getNeutralPlayerNames ()
            |> Command.execute ctxProvider

        member x.createGameAndAddPlayer (gameRequest, playerRequest) =
            let cmd1 = Commands2.createGame gameRequest
            let getCmd2 = fun gameId -> Commands2.addPendingPlayer (gameId, playerRequest)
            CommandProcessor.executeTransactionallyButReturnFirstResult (cmd1, getCmd2) ctxProvider