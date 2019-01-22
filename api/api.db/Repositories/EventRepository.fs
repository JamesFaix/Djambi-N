module Djambi.Api.Db.Repositories.EventRepository

open System.Linq
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
    
let persistEvent (oldGame : Game, newGame : Game) : Game AsyncHttpResult =
    
    //TODO: open transaction

    //remove players
    okTask ()
    |> thenBindAsync (fun _ ->
        let removedPlayers = 
            oldGame.players 
            |> Seq.filter (fun oldP -> 
                newGame.players 
                |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))

        okTask removedPlayers
        |> thenDoEachAsync (fun p ->
            GameRepository.removePlayer p.id
        )
    )

    //add players
    |> thenBindAsync (fun _ ->
        let addedPlayers = 
            newGame.players
            |> Seq.filter (fun newP ->
                oldGame.players
                |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))
           
        okTask addedPlayers
        |> thenDoEachAsync (fun p -> 
            let request = 
                {
                    kind = p.kind
                    userId = p.userId
                    name = if p.kind = PlayerKind.User then None else Some p.name
                }
            GameRepository.addPlayer (newGame.id, request)
            |> thenMap ignore
        )    
    )

    //update players
    |> thenBindAsync (fun _ -> 
        let modifiedPlayers =
            Enumerable.Join(
                oldGame.players, newGame.players,
                (fun p -> p.id), (fun p -> p.id),
                (fun _ newP -> newP)
            )

        okTask modifiedPlayers
        |> thenDoEachAsync GameRepository.updatePlayer
    )

    //update game
    |> thenBindAsync (fun _ ->
        if oldGame.parameters <> newGame.parameters
            || oldGame.currentTurn <> newGame.currentTurn
            || oldGame.pieces <> newGame.pieces
            || oldGame.turnCycle <> newGame.turnCycle
            || oldGame.status <> newGame.status
        then GameRepository.updateGame newGame
        else okTask ()
    )

    //TODO: close transaction

    //get and return game
    |> thenBindAsync (fun _ -> 
        GameRepository.getGame newGame.id
    )