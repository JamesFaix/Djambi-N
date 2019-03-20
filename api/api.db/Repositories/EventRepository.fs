namespace Djambi.Api.Db.Repositories

open System
open System.Linq
open Dapper
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Model
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db

type EventRepository(gameRepo : GameRepository) =            
    let getCommands (request : CreateEventRequest, oldGame : Game, newGame : Game) : CommandDefinition seq = 
        let commands = new ArrayList<CommandDefinition>()

        //remove players
        let removedPlayers = 
            oldGame.players 
            |> Seq.filter (fun oldP -> 
                newGame.players 
                |> (not << Seq.exists (fun newP -> oldP.id = newP.id )))
            
        for p in removedPlayers do
            commands.Add (Commands.removePlayer (oldGame.id, p.id))

        //add players
        let addedPlayers = 
            newGame.players
            |> Seq.filter (fun newP ->
                oldGame.players
                |> (not << Seq.exists (fun oldP -> oldP.id = newP.id)))

        for p in addedPlayers do
            commands.Add (Commands2.addFullPlayer p)

        //update players
        let modifiedPlayers =
            Enumerable.Join(
                oldGame.players, newGame.players,
                (fun p -> p.id), (fun p -> p.id),
                (fun _ newP -> newP)
            )
        
        for p in modifiedPlayers do
            commands.Add (Commands2.updatePlayer p)
 
        //update game
        if oldGame.parameters <> newGame.parameters
            || oldGame.currentTurn <> newGame.currentTurn
            || oldGame.pieces <> newGame.pieces
            || oldGame.turnCycle <> newGame.turnCycle
            || oldGame.status <> newGame.status
        then
            commands.Add (Commands2.updateGame newGame) 
        else ()
    
        commands.Add (Commands2.createEvent (oldGame.id, request))

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
            let cmd = Commands2.getEvents (gameId, query)
            queryMany<EventSqlModel>(cmd, "Event")
            |> thenMap (List.map Mapping.mapEventResponse)