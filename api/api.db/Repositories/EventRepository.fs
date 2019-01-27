module Djambi.Api.Db.Repositories.EventRepository

open System
open System.Data
open System.Linq
open Dapper
open FSharp.Control.Tasks
open Newtonsoft.Json
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Model
        
let getEvents (gameId : int, query : EventsQuery) : Event list AsyncHttpResult =
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("Ascending", query.direction |> mapResultsDirectionToAscendingBool)
                    .addOption("MaxResults", query.maxResults)
                    .addOption("ThresholdTime", query.thresholdTime)
                    .addOption("ThresholdEventId", query.thresholdEventId)
    let cmd = proc("Events_Get", param)

    queryMany<EventSqlModel>(cmd, "Event")
    |> thenMap (List.map mapEventResponse)
   
let private getCreateEventCommand (gameId : int, request : CreateEventRequest) : CommandDefinition = 
    let param = DynamicParameters()
                    .add("GameId", gameId)
                    .add("EventKindId", request.kind |> mapEventKindToId)
                    .add("CreatedByUserId", request.createdByUserId)
                    .add("EffectsJson", request.effects |> JsonConvert.SerializeObject)
    proc("Events_Create", param)

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
        commands.Add (GameRepository.getAddFullPlayerCommand p)

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

let persistEvent (request : CreateEventRequest, oldGame : Game, newGame : Game) : StateAndEventResponse AsyncHttpResult =
    task {
        use conn = SqlUtility.getConnection()
        use tran = conn.BeginTransaction()
        let commands = getCommands (oldGame, newGame, tran)   
    
        try 
            for cmd in commands do
                let! _ = conn.ExecuteAsync cmd
                ()

            let cmd = getCreateEventCommand(oldGame.id, request) 
                      |> CommandDefinition.withTransaction tran
            let! eventId = conn.ExecuteScalarAsync<int> cmd

            tran.Commit()

            let event : Event = 
                {
                    id = eventId
                    createdByUserId = request.createdByUserId
                    createdOn = DateTime.UtcNow
                    kind = request.kind
                    effects = request.effects
                }

            return Ok event
        with 
        | _ as ex -> return Error <| (SqlUtility.catchSqlException ex "Effect")
    }
    |> thenBindAsync (fun event -> 
        GameRepository.getGame newGame.id
        |> thenMap (fun game -> 
            {
                game = game
                event = event
            })
    )