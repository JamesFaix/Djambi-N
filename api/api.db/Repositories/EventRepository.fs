namespace Djambi.Api.Db.Repositories

open System
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model

type EventRepository(ctxProvider : CommandContextProvider, 
                     gameRepo : GameRepository) =

    let getMostCommands (oldGame : Game, newGame : Game) : unit ExecutableCommand seq = 
        let commands = new ArrayList<unit ExecutableCommand>()

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
            commands.Add (Commands2.addFullPlayer p |> Command.ignore)

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
    
        commands |> Enumerable.AsEnumerable
        
    interface IEventRepository with
        member x.persistEvent (request, oldGame, newGame) =
            let mostCommands = getMostCommands (oldGame, newGame)
            let lastCommand = Commands2.createEvent (oldGame.id, request)

            SqlUtility.executeTransactionallyAndReturnLastResult mostCommands lastCommand ctxProvider
            |> thenBindAsync (fun eventId -> 
                let e = {
                    id = eventId
                    createdByUserId = request.createdByUserId
                    createdOn = DateTime.UtcNow
                    actingPlayerId = request.actingPlayerId
                    kind = request.kind
                    effects = request.effects
                }

                (gameRepo :> IGameRepository).getGame newGame.id
                |> thenMap (fun game -> { game = game; event = e })
            )
    
        member x.getEvents (gameId, query) =
            let cmd = Commands2.getEvents (gameId, query)
            (cmd.execute ctxProvider)
            |> thenMap (List.map Mapping.mapEventResponse)