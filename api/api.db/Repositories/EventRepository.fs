module Djambi.Api.Db.Repositories.EventRepository

open System.Linq
open System.Transactions
open Dapper
open Newtonsoft.Json
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
        
let updateGame(game : Game) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", game.id)
                    .addOption("Description", game.parameters.description)
                    .add("AllowGuests", game.parameters.allowGuests)
                    .add("IsPublic", game.parameters.isPublic)
                    .add("RegionCount", game.parameters.regionCount)
                    .add("GameStatusId", game.status |> mapGameStatusToId)
                    .add("PiecesJson", JsonConvert.SerializeObject(game.pieces))
                    .add("CurrentTurnJson", JsonConvert.SerializeObject(game.currentTurn))
                    .add("TurnCycleJson", JsonConvert.SerializeObject(game.turnCycle))
    let cmd = proc("Games_Update", param)
    queryUnit(cmd, "Game")
    
let updatePlayer(player : Player) : Unit AsyncHttpResult =
    let param = DynamicParameters()
                    .add("PlayerId", player.id)
                    .addOption("ColorId", player.colorId)
                    .addOption("StartingTurnNumber", player.startingTurnNumber)
                    .addOption("StartingRegion", player.startingRegion)
                    .addOption("IsAlive", player.isAlive)
    let cmd = proc("Players_Update", param)
    queryUnit(cmd, "Player")

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
        |> thenDoEachAsync updatePlayer
    )

    //update game
    |> thenBindAsync (fun _ ->
        if oldGame.parameters <> newGame.parameters
            || oldGame.currentTurn <> newGame.currentTurn
            || oldGame.pieces <> newGame.pieces
            || oldGame.turnCycle <> newGame.turnCycle
            || oldGame.status <> newGame.status
        then updateGame newGame
        else okTask ()
    )

    //TODO: close transaction

    //get and return game
    |> thenBindAsync (fun _ -> 
        GameRepository.getGame newGame.id
    )