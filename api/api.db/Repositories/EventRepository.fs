namespace Djambi.Api.Db.Repositories

open System
open System.Linq
open Dapper
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Common.Json
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Model
open Djambi.Api.Db.Interfaces

type EventRepository(gameRepo : GameRepository) =        
    let getCreateEventCommand (gameId : int, request : CreateEventRequest) : CommandDefinition = 
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("EventKindId", request.kind |> mapEventKindToId)
                        .add("CreatedByUserId", request.createdByUserId)
                        .addOption("ActingPlayerId", request.actingPlayerId)
                        .add("EffectsJson", JsonUtility.serialize request.effects)
        proc("Events_Create", param)

    let getCommands (request : CreateEventRequest, oldGame : Game, newGame : Game) : CommandDefinition seq = 
        let commands = new ArrayList<CommandDefinition>()

        //remove players
        let removedPlayers = 
            oldGame.players 
            |> Seq.filter (fun oldP -> 
                newGame.players 
                |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))
            
        for p in removedPlayers do
            commands.Add (gameRepo.getRemovePlayerCommand (oldGame.id, p.id))

        //add players
        let addedPlayers = 
            newGame.players
            |> Seq.filter (fun newP ->
                oldGame.players
                |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))

        for p in addedPlayers do
            commands.Add (gameRepo.getAddFullPlayerCommand p)

        //update players
        let modifiedPlayers =
            Enumerable.Join(
                oldGame.players, newGame.players,
                (fun p -> p.id), (fun p -> p.id),
                (fun _ newP -> newP)
            )
        
        for p in modifiedPlayers do
            commands.Add (gameRepo.getUpdatePlayerCommand p)
 
        //update game
        if oldGame.parameters <> newGame.parameters
            || oldGame.currentTurn <> newGame.currentTurn
            || oldGame.pieces <> newGame.pieces
            || oldGame.turnCycle <> newGame.turnCycle
            || oldGame.status <> newGame.status
        then
            commands.Add (gameRepo.getUpdateGameCommand newGame) 
        else ()
    
        commands.Add (getCreateEventCommand(oldGame.id, request))

        commands |> Enumerable.AsEnumerable
        
    interface IEventRepository with
        member x.persistEvent (request, oldGame, newGame) =
            let commands = getCommands (request, oldGame, newGame)
            executeTransactionallyAndReturnLastResult commands "Event"
            |> thenBindAsync (fun eventId -> 
                (gameRepo :> IGameRepository).getGame newGame.id
                |> thenMap (fun game -> 
                    {
                        game = game
                        event = {
                            id = eventId
                            createdByUserId = request.createdByUserId
                            createdOn = DateTime.UtcNow
                            actingPlayerId = request.actingPlayerId
                            kind = request.kind
                            effects = request.effects
                        }
                    })
            )
    
        member x.getEvents (gameId, query) =
            let param = DynamicParameters()
                            .add("GameId", gameId)
                            .add("Ascending", query.direction |> mapResultsDirectionToAscendingBool)
                            .addOption("MaxResults", query.maxResults)
                            .addOption("ThresholdTime", query.thresholdTime)
                            .addOption("ThresholdEventId", query.thresholdEventId)
            let cmd = proc("Events_Get", param)

            queryMany<EventSqlModel>(cmd, "Event")
            |> thenMap (List.map mapEventResponse)