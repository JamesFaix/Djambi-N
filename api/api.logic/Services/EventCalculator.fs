﻿//This module determines the event + effects to be processed based on a request
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

//TODO: Add integration tests
let removePlayer (gameId : int, playerId : int) (session : Session) : Event AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        match game.status with
        | Aborted | AbortedWhilePending | Finished -> 
            Error <| HttpException(400, "Cannot remove players from finished or aborted games.")
        | _ ->
            match game.players |> List.tryFind (fun p -> p.id = playerId) with
            | None -> Error <| HttpException(404, "Player not found.")
            | Some player ->
                match player.userId with
                | None -> Error <| HttpException(400, "Cannot remove neutral players from game.")
                | Some x ->
                    if not <| (session.isAdmin
                        || game.createdByUserId = session.userId
                        || x = session.userId)
                    then Error <| HttpException(403, "Cannot remove other users from game.")        
                    else 
                        let effects = new ArrayList<EventEffect>()

                        let playerIdsToRemove =
                            match player.kind with 
                            | User -> 
                                game.players 
                                |> List.filter (fun p -> p.userId = player.userId) 
                                |> List.map (fun p -> p.id)
                            | Guest -> [playerId]
                            | _ -> List.empty //Already eliminated this case in validation above

                        effects.Add(EventEffect.playersRemoved(playerIdsToRemove))

                        //Cancel game if Pending and creator quit
                        if game.status = GameStatus.Pending
                            && game.createdByUserId = player.userId.Value
                            && player.kind = PlayerKind.User
                        then 
                            effects.Add(EventEffect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending))
                        else ()

                        if player.userId = Some session.userId
                        then Ok <| Event.playerQuit(effects |> Seq.toList)
                        else Ok <| Event.playerEjected(effects |> Seq.toList)
    )