namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open Djambi.Api.Http.HttpUtility

module LobbyController =
    
    let getOpenGames : HttpHandler =
        let func ctx =
            LobbyRepository.getOpenGames()
            |> Task.map (List.map mapLobbyGameResponse)
        handle func
            
    let getGames : HttpHandler =
        let func ctx =
            LobbyRepository.getGames()
            |> Task.map (List.map mapLobbyGameResponse)
        handle func

    let createGame : HttpHandler =
        let func (ctx : HttpContext) =
            ctx.BindModelAsync<CreateGameJsonModel>()
            |> Task.map mapCreateGameRequest
            |> Task.bind LobbyRepository.createGame
            |> Task.map mapLobbyGameResponse
        handle func        
    
    let deleteGame(gameId : int) =
        let func ctx = 
            LobbyRepository.deleteGame gameId
        handle func
 
    let addPlayerToGame(gameId : int, userId : int) =
        let func ctx =
            LobbyRepository.addPlayerToGame(gameId, userId)
        handle func

    let removePlayerFromGame(gameId : int, userId : int) =
        let func ctx =
            LobbyRepository.removePlayerFromGame(gameId, userId)
        handle func