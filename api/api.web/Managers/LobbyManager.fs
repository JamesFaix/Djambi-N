module Djambi.Api.Web.Managers.LobbyManager

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.Mappings.LobbyWebMapping
open Djambi.Api.Web.Mappings.GameWebMapping
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.Model.SessionModel
open Djambi.Api.Web.Model.GameWebModel

let getLobbies (jsonModel : LobbiesQueryJsonModel) (session : Session) : LobbyResponseJsonModel list AsyncHttpResult =
    LobbyService.getLobbies (mapLobbiesQuery jsonModel) session
    |> thenMap (List.map mapLobbyResponse)

let getLobby (lobbyId : int) (session : Session) : LobbyWithPlayersResponseJsonModel AsyncHttpResult =
    LobbyService.getLobby lobbyId session
    |> thenMap mapLobbyWithPlayersResponse

let createLobby (jsonModel : CreateLobbyJsonModel) (session : Session) : LobbyResponseJsonModel AsyncHttpResult =
    let model = mapCreateLobbyRequest (jsonModel, session.userId)
    LobbyService.createLobby model session
    |> thenMap mapLobbyResponse

let deleteLobby (lobbyId : int) (session : Session) : Unit AsyncHttpResult =
    LobbyService.deleteLobby lobbyId session
 
let startGame (lobbyId: int) (session : Session) : GameStartResponseJsonModel AsyncHttpResult =
    GameStartService.startGame lobbyId session
    |> thenMap mapGameStartResponseToJson