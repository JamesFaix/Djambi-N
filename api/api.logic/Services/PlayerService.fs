module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let getGamePlayers (gameId : int) (session : Session) : Player list AsyncHttpResult =
    GameRepository.getPlayersForGames [gameId]
    
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