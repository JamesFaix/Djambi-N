module Djambi.Api.Logic.Managers.GameManager

open Djambi.Api.Logic.Services
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Db.Repositories

let private isGameViewableByActiveUser (session : Session) (game : Game) : bool =
    game.parameters.isPublic
    || game.createdByUserId = session.userId
    || game.players |> List.exists(fun p -> p.userId = Some session.userId)

let getGames (query : GamesQuery) (session : Session) : Game list AsyncHttpResult =
    GameRepository.getGames query
    |> thenMap (fun games ->
        if session.isAdmin
        then games
        else games |> List.filter (isGameViewableByActiveUser session)
    )

//TODO: Requires integration tests
let getGame (gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if isGameViewableByActiveUser session game
        then Ok <| game
        else Error <| HttpException(404, "Game not found.")        
    )

let createGame (parameters : GameParameters) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameCrudService.getCreateGameEvent parameters session
    |> Result.bindAsync (EventProcessor.processEvent None)

//TODO: Requires integration tests
let updateGameParameters (gameId : int) (parameters : GameParameters) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        GameCrudService.getUpdateGameParametersEvent (game, parameters) session
        |> Result.bindAsync (EventProcessor.processEvent (Some game))
    )

let addPlayer (gameId : int) (request : CreatePlayerRequest) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        PlayerService.getAddPlayerEvent (game, request) session
        |> Result.bindAsync (EventProcessor.processEvent (Some game))
    )

let removePlayer (gameId : int, playerId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        PlayerService.getRemovePlayerEvent (game, playerId) session
        |> Result.bindAsync (EventProcessor.processEvent (Some game))
    )

let startGame (gameId: int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        GameStartService.getGameStartEvent game session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )

let selectCell (gameId : int, cellId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        TurnService.getCellSelectedEvent (game, cellId) session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )

let resetTurn (gameId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        TurnService.getResetTurnEvent game session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )

let commitTurn (gameId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        TurnService.getCommitTurnEvent game session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )