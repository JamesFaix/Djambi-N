module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let getGamePlayers (gameId : int) (session : Session) : Player list AsyncHttpResult =
    GameRepository.getPlayersForGames [gameId]

let addPlayer (gameId : int, request : CreatePlayerRequest) (session : Session) : Player AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if game.status <> GameStatus.Pending
        then Error <| HttpException(400, "Can only add players to pending games.")
        else
            match request.kind with
            | PlayerKind.User ->
                if request.userId.IsNone
                then Error <| HttpException(400, "UserID must be provided when adding a user player.")
                elif request.name.IsSome
                then Error <| HttpException(400, "Cannot provide name when adding a user player.")
                elif not session.isAdmin && request.userId.Value <> session.userId
                then Error <| HttpException(403, "Cannot add other users to a game.")
                else Ok ()

            | PlayerKind.Guest ->
                if not game.parameters.allowGuests
                then Error <| HttpException(400, "Game does not allow guest players.")
                elif request.userId.IsNone
                then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
                elif request.name.IsNone
                then Error <| HttpException(400, "Must provide name when adding a guest player.")
                elif not session.isAdmin && request.userId.Value <> session.userId
                then Error <| HttpException(403, "Cannot add guests for other users to a game.")
                else Ok ()

            | PlayerKind.Neutral ->
                Error <| HttpException(400, "Cannot directly add neutral players to a game.")
    )
    |> thenBindAsync (fun _ -> GameRepository.addPlayer (gameId, request))

let removePlayer (gameId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    GameRepository.getGame gameId //TODO: This will error if game already started, change to allow quitting
    |> thenBindAsync (fun game ->
        match game.status with
        | Aborted | AbortedWhilePending | Finished -> 
            errorTask <| HttpException(400, "Cannot remove players from finished or aborted games.")
        | _ ->
            match game.players |> List.tryFind (fun p -> p.id = playerId) with
            | None -> errorTask <| HttpException(404, "Player not found.")
            | Some player ->
                match player.userId with
                | None -> errorTask <| HttpException(400, "Cannot remove neutral players from game.")
                | Some x ->
                    if not <| (session.isAdmin
                        || game.createdByUserId = session.userId
                        || x = session.userId)
                    then errorTask <| HttpException(403, "Cannot remove other users from game.")        
                    else 
                        GameRepository.removePlayer playerId
                        |> thenBindAsync (fun _ -> 
                            //Cancel game if Pending and creator quit
                            if game.status = GameStatus.Pending
                                && game.createdByUserId = player.userId.Value
                                && player.kind = PlayerKind.User
                            then 
                                let request : UpdateGameStateRequest =
                                    {
                                        gameId = gameId
                                        status = GameStatus.AbortedWhilePending
                                        pieces = List.empty
                                        turnCycle = List.empty
                                        currentTurn = None
                                    }
                                GameRepository.updateGameState request
                            else okTask ()
                        )
    )

let fillEmptyPlayerSlots (game : Game) : Game AsyncHttpResult =
    let missingPlayerCount = game.parameters.regionCount - game.players.Length

    let getNeutralPlayerNamesToUse (possibleNames : string list) =
        Enumerable.Except(
            possibleNames,
            game.players |> Seq.map (fun p -> p.name),
            StringComparer.OrdinalIgnoreCase)
        |> Utilities.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then game |> okTask
    else
        GameRepository.getNeutralPlayerNames()
        |> thenMap getNeutralPlayerNamesToUse
        |> thenDoEachAsync (fun name ->
            let request = CreatePlayerRequest.neutral (name)
            GameRepository.addPlayer (game.id, request)
            |> thenMap ignore
        )
        |> thenBindAsync (fun _ -> GameRepository.getPlayersForGames [game.id])
        |> thenMap (fun players -> { game with players = players })