namespace Apex.Api.Logic.Services

open Apex.Api.Common.Collections
open Apex.Api.Common.Control
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums

type PlayerStatusChangeService(eventServ : EventService,
                               indirectEffectsServ : IndirectEffectsService) =

    let getFinalAcceptDrawEffects(game : Game, request : PlayerStatusChangeRequest) : Effect list =
        let otherLivingPlayersNotAcceptingADraw =
            game.players
            |> List.filter (fun p ->
                p.id <> request.playerId &&
                p.status = PlayerStatus.Alive
            )

        if otherLivingPlayersNotAcceptingADraw.IsEmpty
        then
            [
                Effect.GameStatusChanged { oldValue = GameStatus.InProgress; newValue = GameStatus.Over }
            ]
        else [] //If not last player to accept, nothing special happens

    member x.getUpdatePlayerStatusEvent (game : Game, request : PlayerStatusChangeRequest) (session : Session) : CreateEventRequest HttpResult =
        if game.status <> GameStatus.InProgress then
            Error <| HttpException(400, "Cannot change player status unless game is InProgress.")
        else
            Security.ensurePlayerOrHas Privilege.OpenParticipation session game
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
                    | (PlayerStatus.Alive, PlayerStatus.AcceptsDraw) ->
                        let finalAcceptDrawEffects = getFinalAcceptDrawEffects (game, request)
                        Ok { event with effects = primaryEffect :: finalAcceptDrawEffects }
                    | (PlayerStatus.AcceptsDraw, PlayerStatus.Alive) ->
                        //If revoking draw, just change status
                        Ok event
                    | (PlayerStatus.Alive, PlayerStatus.Conceded)
                    | (PlayerStatus.AcceptsDraw, PlayerStatus.Conceded)
                    | (PlayerStatus.Alive, PlayerStatus.WillConcede)
                    | (PlayerStatus.AcceptsDraw, PlayerStatus.WillConcede) ->
                        let isPlayersTurn = game.turnCycle.[0] = request.playerId
                        if isPlayersTurn
                        then
                            let effects = new ArrayList<Effect>()

                            let primary = Effect.PlayerStatusChanged {
                                oldStatus = oldStatus
                                newStatus = PlayerStatus.Conceded
                                playerId = player.id
                            }

                            effects.Add(primary)
                            let game = eventServ.applyEffect primary game
                            effects.AddRange (indirectEffectsServ.getIndirectEffectsForConcede (game, request))
                            Ok { event with effects = effects |> Seq.toList }
                        else
                            let primary = Effect.PlayerStatusChanged {
                                oldStatus = oldStatus
                                newStatus = PlayerStatus.WillConcede
                                playerId = player.id
                            }
                            Ok { event with effects = [primary] }

                    | _ ->
                        Error <| HttpException(400, "Player status transition not allowed.")
            )