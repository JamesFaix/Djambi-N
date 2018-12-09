module Djambi.Api.Web.Managers.LobbyManager

open Djambi.Api.Common
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let getLobbies (query : LobbiesQuery) (session : Session) : Lobby list AsyncHttpResult =
    LobbyService.getLobbies query session

let getLobby (lobbyId : int) (session : Session) : LobbyWithPlayers AsyncHttpResult =
    LobbyService.getLobby lobbyId session

let createLobby (request : CreateLobbyRequest) (session : Session) : Lobby AsyncHttpResult =
    LobbyService.createLobby request session

let deleteLobby (lobbyId : int) (session : Session) : Unit AsyncHttpResult =
    LobbyService.deleteLobby lobbyId session
 
let startGame (lobbyId: int) (session : Session) : StartGameResponse AsyncHttpResult =
    GameStartService.startGame lobbyId session