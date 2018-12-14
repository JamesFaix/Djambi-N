module Djambi.Api.Logic.Managers.GameManager

open Djambi.Api.Logic.Services
open Djambi.Api.Common
open Djambi.Api.Model
open Djambi.Api.Common.AsyncHttpResult
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
    EventCalculator.createGame parameters session
    |> thenBindAsync (EventProcessor.processEvent None)

//TODO: Requires integration tests
let updateGameParameters (gameId : int) (parameters : GameParameters) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameService.getGame gameId session
    |> thenBindAsync (fun game -> 
        EventCalculator.updateGameParameters (gameId, parameters) session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )

let addPlayer (gameId : int) (request : CreatePlayerRequest) (session : Session) : StateAndEventResponse AsyncHttpResult =
    GameService.getGame gameId session
    |> thenBindAsync (fun game -> 
        EventCalculator.addPlayer (gameId, request) session
        |> thenBindAsync (EventProcessor.processEvent (Some game))
    )

let removePlayer (gameId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    PlayerService.removePlayer (gameId, playerId) session

let startGame (gameId: int) (session : Session) : Game AsyncHttpResult =
    GameStartService.startGame gameId session

let selectCell (gameId : int, cellId : int) (session : Session) : Turn AsyncHttpResult =
    TurnService.selectCell (gameId, cellId) session

let resetTurn (gameId : int) (session : Session) : Turn AsyncHttpResult =
    TurnService.resetTurn gameId session

let commitTurn (gameId : int) (session : Session) : Game AsyncHttpResult =
    TurnService.commitTurn gameId session