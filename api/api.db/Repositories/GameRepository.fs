namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model
open System

type GameRepository(ctxProvider : CommandContextProvider) =
    let getGameWithoutPlayers (gameId : int) : Game AsyncHttpResult =
        Commands.getGame gameId
        |> Command.execute ctxProvider
        |> thenMap Mapping.mapGameResponse

    let getPlayersForGame (gameId : int) : Player List AsyncHttpResult =
        Commands2.getPlayersForGame gameId
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
                getPlayersForGame gameId
                |> thenMap (fun players -> { game with players = players })
            )

        [<Obsolete("Only used for tests")>]
        member x.createGame request =
            Commands2.createGame request
            |> Command.execute ctxProvider

        [<Obsolete("Only used for tests")>]
        member x.addPlayer (gameId, request) =
            Commands2.addPendingPlayer (gameId, request)
            |> Command.execute ctxProvider
            |> thenBindAsync (fun pId -> getPlayer (gameId, pId))

        [<Obsolete("Only used for tests")>]
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