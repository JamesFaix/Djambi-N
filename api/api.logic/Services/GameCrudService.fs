module Djambi.Api.Logic.Services.GameCrudService

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model
open Djambi.Api.Logic
    
let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    let self = session.user
    let gameRequest : CreateGameRequest =   
        {
            parameters = parameters
            createdByUserId = self.id
        }

    let playerRequest : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some self.id
            name = None
        }

    GameRepository.createGameAndAddPlayer (gameRequest, playerRequest)
    |> thenBindAsync GameRepository.getGame

let getUpdateGameParametersEvent (game : Game, parameters : GameParameters) (session : Session) : CreateEventRequest HttpResult =
    if game.status <> GameStatus.Pending
    then Error <| HttpException (400, "Cannot change game parameters unless game is Pending.")
    else Ok ()
    |> Result.bind (fun _ -> SecurityService.ensureAdminOrCreator session game)
    |> Result.map (fun _ ->
        let effects = new ArrayList<Effect>()

        effects.Add(Effect.ParametersChanged { oldValue = game.parameters; newValue = parameters })

        //If lowering region count, extra players are ejected
        let truncatedPlayers = 
            game.players 
            |> Seq.skipSafe parameters.regionCount 

        //If disabling AllowGuests, guests are ejected
        let ejectedGuests =
            if parameters.allowGuests = false
            then game.players
                |> Seq.filter (fun p -> p.kind = PlayerKind.Guest)
            else Seq.empty

        let removedPlayerIds = 
            truncatedPlayers 
            |> Seq.append ejectedGuests 
            |> Seq.map (fun p -> p.id)
            |> Seq.distinct
            |> Seq.toList

        for pId in removedPlayerIds do
            effects.Add(Effect.PlayerRemoved({playerId = pId}))

        {
            kind = EventKind.GameParametersChanged
            effects = effects |> Seq.toList
            createdByUserId = session.user.id
            actingPlayerId = ContextService.getActingPlayerId session game
        }
    )