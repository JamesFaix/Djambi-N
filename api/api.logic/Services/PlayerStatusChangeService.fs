module Djambi.Api.Logic.Services.PlayerStatusChangeService

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Model

let private getFinalAcceptDrawEffects(game : Game, request : PlayerStatusChangeRequest) : Effect list =
    let otherLivingPlayers = 
        game.players 
        |> List.filter (fun p -> 
            p.id <> request.playerId &&
            p.status = Alive
        )

    let nonNeutralPlayers = 
        otherLivingPlayers
        |> List.filter (fun p -> p.userId.IsSome)

    let neutralPlayers = 
        otherLivingPlayers
        |> List.filter (fun p -> p.userId.IsNone)

    if nonNeutralPlayers.IsEmpty
    then
        //If accepting draw, and everyone has accepted
        let neutralPlayerConcedeEffects = 
            neutralPlayers 
            |> List.map (fun p -> 
                Effect.PlayerStatusChanged{ 
                    playerId = p.id
                    oldStatus=p.status
                    newStatus=Conceded
                }
            )

        let finalEffect = Effect.GameStatusChanged { oldValue = Started; newValue = Finished }
        let fx = List.append neutralPlayerConcedeEffects [finalEffect]
        fx
    else [] //If not last player to accept, nothing special happens
   
let getUpdatePlayerStatusEvent (game : Game, request : PlayerStatusChangeRequest) (session : Session) : CreateEventRequest HttpResult = 
    if game.status <> Started then
        Error <| HttpException(400, "Cannot change player status unless game is Started.")
    else    
        SecurityService.ensurePlayerOrHas OpenParticipation session game
        |> Result.bind (fun _ ->
            let player = game.players |> List.find (fun p -> p.id = request.playerId)
            let oldStatus = player.status
            let newStatus = request.status

            if oldStatus = newStatus
                then Error <| HttpException(400, "Cannot change player status to current status.")
            else 
                let primaryEffect = Effect.PlayerStatusChanged { 
                    oldStatus = oldStatus
                    newStatus = newStatus
                    playerId = player.id 
                } 

                let event = {
                    kind = EventKind.PlayerStatusChanged
                    effects = [ primaryEffect ]
                    createdByUserId = session.user.id
                    actingPlayerId = Some request.playerId
                }

                match (oldStatus, newStatus) with
                | (Alive, AcceptsDraw) ->
                    let finalAcceptDrawEffects = getFinalAcceptDrawEffects (game, request)
                    Ok { event with effects = primaryEffect :: finalAcceptDrawEffects }
                | (AcceptsDraw, Alive) ->
                    //If revoking draw, just change status
                    Ok event
                | (Alive, Conceded)
                | (AcceptsDraw, Conceded)
                | (Alive, WillConcede)
                | (AcceptsDraw, WillConcede) ->
                    let isPlayersTurn = game.turnCycle.[0] = request.playerId
                    if isPlayersTurn
                    then
                        let primary = Effect.PlayerStatusChanged { 
                            oldStatus = oldStatus
                            newStatus = Conceded
                            playerId = player.id 
                        } 
                        //Remove from turn cycle
                        //Abandon pieces
                        //Check for victory and draw
                        //End turn
                        //Check for player out of moves

                        let fx = [primary]
                        Ok { event with effects = fx }
                    else
                        let primary = Effect.PlayerStatusChanged { 
                            oldStatus = oldStatus
                            newStatus = WillConcede
                            playerId = player.id 
                        } 
                        Ok { event with effects = [primary] }

                | _ -> 
                    Error <| HttpException(400, "Player status transition not allowed.")
        )