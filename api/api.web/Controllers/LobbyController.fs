module Djambi.Api.Web.Controllers.LobbyController

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Model.LobbyWebModel

let getOpenGames : HttpHandler =
    let func ctx =
        LobbyRepository.getOpenGames()
        |> thenMap (List.map mapLobbyGameResponse)
    handle func
            
let getGames : HttpHandler =
    let func ctx =
        LobbyRepository.getGames()
        |> thenMap (List.map mapLobbyGameResponse)
    handle func

let getUserGames (userId : int) =
    let func ctx =
        LobbyRepository.getUserGames userId
        |> thenMap (List.map mapLobbyGameResponse)
    handle func
    
let createGame : HttpHandler =
    let func (ctx : HttpContext) : LobbyGameJsonModel AsyncHttpResult =
        getSessionAndModelFromContext<CreateGameJsonModel> ctx 
        |> thenBindAsync (fun (requestJsonModel, session) -> 
            let request = mapCreateGameRequest (requestJsonModel, session)
            LobbyService.createGame (request, session))
        |> thenMap mapLobbyGameResponse
    handle func    

let deleteGame(gameId : int) =
    let func ctx = 
        getSessionFromContext ctx
        |> thenBindAsync (fun session -> 
            LobbyService.deleteGame (gameId, session)
        )
    handle func
 
let addPlayerToGame(gameId : int, userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (fun session -> 
            LobbyService.addPlayerToGame(gameId, userId, session)
        )
    handle func

let removePlayerFromGame(gameId : int, userId : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (fun session -> 
            LobbyService.removePlayerFromGame(gameId, userId, session)
        )
    handle func