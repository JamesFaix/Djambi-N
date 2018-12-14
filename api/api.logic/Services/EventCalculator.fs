//This module determines the event + effects to be processed based on a request
module Djambi.Api.Logic.Services.EventCalculator

open Djambi.Api.Model
open Djambi.Api.Common
open Djambi.Api.Db.Repositories
open Djambi.Api.Common.AsyncHttpResult

type ArrayList<'a> = System.Collections.Generic.List<'a>
    
//TODO: Add integration tests
let createGame (parameters : GameParameters) (session : Session) : Event AsyncHttpResult =
    let gameRequest = 
        {
            parameters = parameters
            createdByUserId = session.userId
        }
    let playerRequest = CreatePlayerRequest.user session.userId
    okTask <| Event.gameCreated (gameRequest, playerRequest)

//TODO: Add integration tests
let updateGameParameters (gameId : int, parameters : GameParameters) (session : Session) : Event AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game -> 
        if game.status <> GameStatus.Pending
        then Error <| HttpException (400, "Cannot change game parameters unless game is Pending.")
        elif not (session.isAdmin || session.userId = game.createdByUserId)
        then Error <| HttpException(403, "Cannot change game parameters of game created by another user.")
        else 
            let effects = new ArrayList<EventEffect>()

            effects.Add(EventEffect.parametersChanged(game.parameters, parameters))

            //If lowering region count, extra players are ejected
            let truncatedPlayers = 
                game.players 
                |> Seq.skip parameters.regionCount 

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

            if removedPlayerIds.Length > 0
            then effects.Add(EventEffect.playersRemoved removedPlayerIds)
            else ()

            Ok <| Event.gameParametersChanged (effects |> Seq.toList)
    )

//TODO: Add integration tests
let addPlayer (gameId : int, request : CreatePlayerRequest) (session : Session) : Event AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if game.status <> GameStatus.Pending
        then Error <| HttpException(400, "Can only add players to pending games.")
        else
            match request.kind with
            | PlayerKind.User ->
                if request.userId.IsNone
                then Error <| HttpException(400, "UserID must be provided when adding a user player.")
                elif request.name.IsSome
                then Error <| HttpException(400, "Cannot provide name when adding a user player.")
                elif not session.isAdmin && request.userId.Value <> session.userId
                then Error <| HttpException(403, "Cannot add other users to a game.")
                else Ok ()

            | PlayerKind.Guest ->
                if not game.parameters.allowGuests
                then Error <| HttpException(400, "Game does not allow guest players.")
                elif request.userId.IsNone
                then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
                elif request.name.IsNone
                then Error <| HttpException(400, "Must provide name when adding a guest player.")
                elif not session.isAdmin && request.userId.Value <> session.userId
                then Error <| HttpException(403, "Cannot add guests for other users to a game.")
                else Ok ()

            | PlayerKind.Neutral ->
                Error <| HttpException(400, "Cannot directly add neutral players to a game.")
    )
    |> thenMap (fun _ -> Event.playerJoined request)