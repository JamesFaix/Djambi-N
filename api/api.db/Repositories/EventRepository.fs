module Djambi.Api.Db.Repositories.EventRepository

open System.Data
open System.Linq
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Model
open Djambi.Api.Db
        
let private getCommands (oldGame : Game, newGame : Game, transaction : IDbTransaction) : CommandDefinition seq = 
    let commands = new ArrayList<CommandDefinition>()

    //remove players
    let removedPlayers = 
        oldGame.players 
        |> Seq.filter (fun oldP -> 
            newGame.players 
            |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))
            
    for p in removedPlayers do
        commands.Add (GameRepository.getRemovePlayerCommand p.id)

    //add players
    let addedPlayers = 
        newGame.players
        |> Seq.filter (fun newP ->
            oldGame.players
            |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))

    for p in addedPlayers do
        let playerRequest = 
            {
                kind = p.kind
                userId = p.userId
                name = if p.kind = PlayerKind.User then None else Some p.name
            }       
        commands.Add (GameRepository.getAddPlayerCommand(oldGame.id, playerRequest))

    //update players
    let modifiedPlayers =
        Enumerable.Join(
            oldGame.players, newGame.players,
            (fun p -> p.id), (fun p -> p.id),
            (fun _ newP -> newP)
        )
        
    for p in modifiedPlayers do
        commands.Add (GameRepository.getUpdatePlayerCommand p)
 
    //update game
    if oldGame.parameters <> newGame.parameters
        || oldGame.currentTurn <> newGame.currentTurn
        || oldGame.pieces <> newGame.pieces
        || oldGame.turnCycle <> newGame.turnCycle
        || oldGame.status <> newGame.status
    then
        commands.Add (GameRepository.getUpdateGameCommand newGame) 
    else ()

    commands 
    |> Enumerable.AsEnumerable 
    |> Seq.map (CommandDefinition.withTransaction transaction)

let persistEvent (oldGame : Game, newGame : Game) : Game AsyncHttpResult =
    task {
        use conn = SqlUtility.getConnection()
        use tran = conn.BeginTransaction()
        let commands = getCommands (oldGame, newGame, tran)   
    
        try 
            for cmd in commands do
                let! _ = conn.ExecuteAsync cmd
                ()
            tran.Commit()
            return Ok ()
        with 
        | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
    }
    |> AsyncHttpResult.thenBindAsync (fun _ -> GameRepository.getGame newGame.id)
    