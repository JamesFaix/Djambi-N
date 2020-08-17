namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Model
open Djambi.Api.Logic
open Djambi.Api.Enums

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

    member x.getUpdatePlayerStatusEvent (game : Game, request : PlayerStatusChangeRequest) (session : Session) : CreateEventRequest =
        if game.status <> GameStatus.InProgress then
            raise <| GameRuleViolationException("Cannot change player status unless game is InProgress.")
        else
            Security.ensurePlayerOrHas Privilege.OpenParticipation session game
            let player = game.players |> List.find (fun p -> p.id = request.playerId)
            let oldStatus = player.status
            let newStatus = request.status

            if oldStatus = newStatus
                then raise <| GameRuleViolationException("Cannot change player status to current status.")
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
                    { event with effects = primaryEffect :: finalAcceptDrawEffects }
                | (PlayerStatus.AcceptsDraw, PlayerStatus.Alive) ->
                    //If revoking draw, just change status
                    event
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
                        { event with effects = effects |> Seq.toList }
                    else
                        let primary = Effect.PlayerStatusChanged {
                            oldStatus = oldStatus
                            newStatus = PlayerStatus.WillConcede
                            playerId = player.id
                        }
                        { event with effects = [primary] }

                | _ ->
                    raise <| GameRuleViolationException("Player status transition not allowed.")