module Djambi.Api.Web.Controllers.LobbyController

open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Mappings.GameWebMapping
open Djambi.Api.Web.Model.LobbyWebModel

let getLobbies : HttpHandler =
    let func ctx =
        getSessionAndModelFromContext<LobbiesQueryJsonModel> ctx
        |> thenBindAsync (fun (query, session) ->
            LobbyService.getLobbies (mapLobbiesQuery query) session
        )
        |> thenMap (List.map mapLobbyResponse)
    handle func
                
let createLobby : HttpHandler =
    let func (ctx : HttpContext) : LobbyResponseJsonModel AsyncHttpResult =
        getSessionAndModelFromContext<CreateLobbyJsonModel> ctx 
        |> thenBindAsync (fun (requestJsonModel, session) -> 
            let request = mapCreateLobbyRequest (requestJsonModel, session.userId)
            LobbyService.createLobby request session)
        |> thenMap mapLobbyResponse
    handle func    

let deleteLobby(lobbyId : int) =
    let func ctx = 
        getSessionFromContext ctx
        |> thenBindAsync (LobbyService.deleteLobby lobbyId)
    handle func

let startGame(lobbyId: int) =
    //Error if not creating user
    let func ctx = 
        getSessionFromContext ctx
        |> thenBindAsync (fun session -> GameStartService.startGame lobbyId)
        |> thenMap mapGameStartResponseToJson
    handle func
